using LiteDB;
using ShareLingo.Core.Model;
using ShareLingo.Core.ViewModel;
using System.IO.Abstractions;

namespace ShareLingo.Core.Services
{
    public class LiteDbManager : IDataManager
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly string filePath;
        private readonly string password = string.Empty;
        private LiteDatabase? connection;
        #endregion

        #region Constructors
        public LiteDbManager(IBuildInfoManager bldMng)
        {
            fileSystem = bldMng.FileSystem;
            filePath = bldMng.DatabasePath;
        }
        #endregion

        #region Methods
        public void Open()
        {
            if (connection != null) throw new ArgumentException("Connection is already opened.");
            connection = new LiteDatabase($"FileName={filePath};Password={password}");
        }


        public CourseContainerViewModel[] GetCourseData(int skip, int limit)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var dataItems = connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).FindAll().Skip(skip).Take(limit);
            var fs = connection.GetStorage<string>();
            var result = new List<CourseContainerViewModel>();
            foreach (var item in dataItems)
            {
                var course = new CourseContainerViewModel(item);
                if (!string.IsNullOrWhiteSpace(item.CoverId))
                {
                    var memStream = new MemoryStream();
                    fs.Download(item.CoverId, memStream);
                    course.Cover = new AttachedFileViewModel(item.CoverId, memStream);
                }
                if (!string.IsNullOrWhiteSpace(item.DescriptionDocumentId))
                {
                    var memStream = new MemoryStream();
                    fs.Download(item.DescriptionDocumentId, memStream);
                    course.Description = new AttachedFileViewModel(item.DescriptionDocumentId, memStream);
                }
            }
            return result.ToArray();
        }
        public ModuleItemViewModel[] GetModulesData(CourseContainerViewModel course, int skip, int limit)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (course == null) throw new ArgumentException("Course must be set.");

            var dataItems = connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Find(x => x.Course.Id == course.Item.Id).Skip(skip).Take(limit);
            var fs = connection.GetStorage<string>();
            var result = new List<ModuleItemViewModel>();
            foreach (var item in dataItems)
            {
                var module = new ModuleItemViewModel(item, course);
                if (!string.IsNullOrWhiteSpace(item.CoverId))
                {
                    var memStream = new MemoryStream();
                    fs.Download(item.CoverId, memStream);
                    module.Cover = new AttachedFileViewModel(item.CoverId, memStream);
                }
                if (!string.IsNullOrWhiteSpace(item.TheoryDocumentId))
                {
                    var memStream = new MemoryStream();
                    fs.Download(item.TheoryDocumentId, memStream);
                    module.Theory = new AttachedFileViewModel(item.TheoryDocumentId, memStream);
                }
                result.Add(module);
            }
            return result.ToArray();
        }
        public ExerciseViewModelBase[] GetExercisesData(ModuleItemViewModel module)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (module == null) throw new ArgumentException("Module must be set.");
            var dataItems = connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Find(x => x.Module.Id == module.Item.Id);
            var result = new List<ExerciseViewModelBase>();
            foreach (var item in dataItems)
            {
                if (item is MissingTextExercise missingText) result.Add(GetMissingTextExercise(missingText, module));
                else throw new NotImplementedException($"Exercise type {item.GetType().Name} is not implemented.");
            }
            return result.ToArray();
        }
        public ExerciseSubItemViewModelBase[] GetItemsToRepeat(CourseContainerViewModel course, int limit)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var tags = connection.GetCollection<Tag>(Tag.COLLECTION_NAME)
                .Include(x => x.Course)
                .Find(x => x.Course.Id == course.Item.Id);

            var bestTagId = -1;
            double bestTagPriority = -1;
            List<ExerciseSubItemViewModelBase> bestItems = new();

            foreach (var tag in tags)
            {
                // Get all relations for this tag
                var tagRelations = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME)
                    .Include(x => x.ExerciseItem).Include(x => x.RelatedTag).Include(x => x.Exercise)
                    .Find(tr => tr.RelatedTag.Id == tag.Id)
                    .Where(tr => tr.ExerciseItem != null)
                    .ToList();

                // Get distinct ExerciseSubItems
                var items = tagRelations
                    .Select(tr => tr.ExerciseItem)
                    .Distinct()
                    .ToList();

                if (items.Count == 0)
                    continue;

                // Compute average priority for this tag group
                var priorities = items
                    .Select(x => x.GetPriotity())
                    .ToList();

                double avgPriority = priorities.Average();

                if (avgPriority > bestTagPriority)
                {
                    bestTagPriority = avgPriority;

                    var _items = items
                        .OrderByDescending(x => x.GetPriotity())
                        .Take(limit)
                        .ToList();
                    foreach(var item in _items)
                    {
                        var tagRelation = tagRelations.FirstOrDefault(x => x.ExerciseItem.Id == item.Id);

                        if (item is MissingTextExerciseSubItem missingTextSubItem) 
                            bestItems.Add(new MissingTextExerciseItemViewModel(missingTextSubItem, null!));
                    }

                    bestTagId = tag.Id;
                }
            }
            return bestItems.ToArray();
        }
        public AttachedTagViewModel? GetTag(string? tagName, ExerciseSubItemViewModelBase subItem)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var tag = connection.GetCollection<Tag>(Tag.COLLECTION_NAME).FindOne(x => x.Name == tagName);
            if (tag == null) return null;
            var tagRelation = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME)
                .FindOne(x => x.ExerciseItem.Id == subItem.Item.Id && x.RelatedTag.Id == tag.Id);
            if (tagRelation != null) return new AttachedTagViewModel(tag, tagRelation, subItem);
            else
            {
                tagRelation = new TagRelation
                {
                    ExerciseItem = subItem.Item,
                    Exercise = subItem.Parent.Item,
                    RelatedTag = tag
                };
                return new AttachedTagViewModel(tag, tagRelation, subItem);
            }
        }

        public void SaveCourseData(CourseContainerViewModel item)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var fs = connection.GetStorage<string>();

            if (item.Item.Id != default) connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Update(item.Item);
            else item.Item.Id = (int)(connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Insert(item.Item));

            if (item.Description != null)
            {
                if (!string.IsNullOrWhiteSpace(item.Item.DescriptionDocumentId))
                    fs.Delete(item.Item.DescriptionDocumentId);
                item.Description.Stream.Seek(0, SeekOrigin.Begin);
                var guid = Guid.NewGuid().ToString().Substring(0,8);
                var fileExt = fileSystem.Path.GetExtension(item.Description.Name);
                var fileName = $"{guid}.{fileExt}";
                var fileId = $"$/docs/courses/{item.Item.Id}/{fileName}";
                fs.Upload(fileId, fileName, item.Description.Stream);
                item.Item.DescriptionDocumentId = fileId;
            }
            if (item.Cover != null)
            {
                if (!string.IsNullOrWhiteSpace(item.Item.CoverId))
                    fs.Delete(item.Item.CoverId);
                item.Cover.Stream.Seek(0, SeekOrigin.Begin);
                var guid = Guid.NewGuid().ToString().Substring(0, 8);
                var fileExt = fileSystem.Path.GetExtension(item.Cover.Name);
                var fileName = $"{guid}.{fileExt}";
                var fileId = $"$/images/courses/{item.Item.Id}/{fileName}";
                fs.Upload(fileId, fileName, item.Cover.Stream);
                item.Item.CoverId = fileId;
            }
            connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Update(item.Item);
        }
        public void SaveModuleData(ModuleItemViewModel item)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (item.Item.Course == null) throw new ArgumentException("Course must be set.");
            var fs = connection.GetStorage<string>();

            if (item.Item.Id != default)
            {
                var modulesWithTheSameIndex = connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME)
                    .Count(x => x.Id != item.Item.Id && x.Course.Id == item.Item.Course.Id && x.Index == item.Item.Index);
                if (modulesWithTheSameIndex > 0)
                    throw new ArgumentException($"Module with index {item.Item.Index} already exists in the course {item.Item.Course.Name}.");
                connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Update(item.Item);
            }
            else
            {
                var modulesWithTheSameIndex = connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME)
                    .Count(x => x.Course.Id == item.Item.Course.Id && x.Index == item.Item.Index);
                if (modulesWithTheSameIndex > 0)
                    throw new ArgumentException($"Module with index {item.Item.Index} already exists in the course {item.Item.Course.Name}.");
                item.Item.Id = (int)(connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Insert(item.Item));
            }

            if (item.Theory != null)
            {
                if (!string.IsNullOrWhiteSpace(item.Item.TheoryDocumentId))
                    fs.Delete(item.Item.TheoryDocumentId);
                item.Theory.Stream.Seek(0, SeekOrigin.Begin);
                var guid = Guid.NewGuid().ToString().Substring(0, 8);
                var fileExt = fileSystem.Path.GetExtension(item.Theory.Name);
                var fileName = $"{guid}.{fileExt}";
                var fileId = $"$/docs/modules/{item.Item.Id}/{fileName}";
                fs.Upload(fileId, fileName, item.Theory.Stream);
                item.Item.TheoryDocumentId = fileId;
            }
            if (item.Cover != null)
            {
                if (!string.IsNullOrWhiteSpace(item.Item.CoverId))
                    fs.Delete(item.Item.CoverId);
                item.Cover.Stream.Seek(0, SeekOrigin.Begin);
                var guid = Guid.NewGuid().ToString().Substring(0, 8);
                var fileExt = fileSystem.Path.GetExtension(item.Cover.Name);
                var fileName = $"{guid}.{fileExt}";
                var fileId = $"$/images/modules/{item.Item.Id}/{fileName}";
                fs.Upload(fileId, fileName, item.Cover.Stream);
                item.Item.CoverId = fileId;
            }
            connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Update(item.Item);
        }
        public void SaveExercise(MissingTextExerciseViewModel exercise)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (exercise.Item.Module == null) throw new ArgumentException("Module must be set.");

            if (exercise.Item.Id != default)
            {
                connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Update(exercise.Item);
                var removedItems = connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME)
                    .Find(x => x.Exercise.Id == exercise.Item.Id)
                    .Where(x => !exercise.Items.Any(c => c.Item.Id == x.Id)).ToArray();
                foreach (var item in removedItems)
                {
                    var tagRelations = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME)
                        .DeleteMany(x => x.ExerciseItem.Id == item.Id);
                    connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Delete(item.Id);
                }
                var removedTagRelations = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME)
                    .Find(x => x.Exercise.Id == exercise.Item.Id)
                    .Where(x => !exercise.Items.Any(c => c.Item.Id == x.ExerciseItem.Id)).ToArray();
                foreach (var item in removedTagRelations) connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME).Delete(item.Id);
            }
            else
                exercise.Item.Id = (int)(connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Insert(exercise.Item));

            foreach (var item in exercise.Items)
            {
                if (item.Item.Id != default) connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Update(item.Item);
                else item.Item.Id = (int)(connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Insert(item.Item));
            }
        }
        public void SaveExerciseProgress(ExerciseViewModelBase exercise)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (exercise.Item.Module == null) throw new ArgumentException("Module must be set.");
            else if (exercise.Item.Id == default) throw new ArgumentException("Exercise must be saved first.");

            var subItems = exercise.GetSubItems();
            foreach(var subItem in subItems)
            {
                if (subItem.Item.Id == default) throw new ArgumentException("Exercise subitem must be saved first.");
                var subItemFromDb = connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).FindById(subItem.Item.Id);
                var basedAdjustment = 0.2d;
                var maxScore = 1.0d;
                double attemptFactor = subItem.AttemptsCount switch { 1 => 1.0d, 2 => 0.8d, 3 => 0.6d, _ => 0.1d };
                var delta = basedAdjustment * attemptFactor;
                subItemFromDb.Score = Math.Min(maxScore, subItemFromDb.Score + delta);
                subItemFromDb.LastAttempt = DateTime.Now;
                subItemFromDb.AttemptCount += subItem.AttemptsCount;
                connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Update(subItem.Item);
            }
        }
        public void SaveTag(AttachedTagViewModel item)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (string.IsNullOrWhiteSpace(item.Tag.Name)) throw new ArgumentException("Tag name must be set.");
            else if (item.ExerciseItem == null) throw new ArgumentException("Exercise must be set.");
            else if (item.ExerciseItem.Item.Id == default) throw new ArgumentException("Exercise must be saved first.");
            else if (item.ExerciseItem.Parent.Item.Id == default) throw new ArgumentException("Exercise parent must be saved first.");
            else if (item.ExerciseItem.Parent.ModuleItem.Item.Id == default) throw new ArgumentException("Module must be saved first.");
            else if (item.ExerciseItem.Parent.ModuleItem.Course.Item.Id == default) throw new ArgumentException("Course must be saved first.");

            if (item.Tag.Id == default)
            {
                var count = connection.GetCollection<Tag>(Tag.COLLECTION_NAME).Count(x => x.Name == item.Tag.Name);
                if (count > 0) throw new Exception($"Tag with name {item.Tag.Name} already exists.");
                item.Tag.Id = connection.GetCollection<Tag>(Tag.COLLECTION_NAME).Insert(item.Tag);
            }
            else
            {
                var count = connection.GetCollection<Tag>(Tag.COLLECTION_NAME).Count(x => x.Name == item.Tag.Name && x.Id != item.Tag.Id);
                if (count > 0) throw new Exception($"Tag with name {item.Tag.Name} already exists.");
                connection.GetCollection<Tag>(Tag.COLLECTION_NAME).Update(item.Tag);
            }

            var tagRelationCount = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME)
                .Count(x => x.Exercise.Id == item.ExerciseItem.Parent.Item.Id
                    && x.ExerciseItem.Id == item.ExerciseItem.Item.Id
                    && x.RelatedTag.Id == item.Tag.Id
                    && x.Id != item.TagRelation.Id);
            if (tagRelationCount > 0) throw new Exception($"Tag relation was already created.");
            if (item.TagRelation.Id == default) item.TagRelation.Id = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME).Insert(item.TagRelation);
            else connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME).Update(item.TagRelation);
        }

        public void DeleteCourseData(int id)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var fs = connection.GetStorage<string>();
            var course = connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).FindById(id);
            if (course == null) throw new ArgumentNullException(nameof(course), "Course not found.");
            if (!string.IsNullOrWhiteSpace(course.DescriptionDocumentId))
                fs.Delete(course.DescriptionDocumentId);
            if (!string.IsNullOrWhiteSpace(course.CoverId))
                fs.Delete(course.CoverId);
            connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Delete(course.Id);
            var relatedModules = connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Find(x => x.Course.Id == course.Id);
            foreach (var module in relatedModules) DeleteModuleData(module.Id);
            var relatedTags = connection.GetCollection<Tag>(Tag.COLLECTION_NAME).Find(x => x.Course.Id == course.Id);
            foreach (var tag in relatedTags) DeleteTag(tag.Id);
        }
        public void DeleteModuleData(int id)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var fs = connection.GetStorage<string>();
            var module = connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).FindById(id);
            if (module == null) throw new ArgumentNullException(nameof(module), "Module not found.");
            if (!string.IsNullOrWhiteSpace(module.CoverId)) fs.Delete(module.CoverId);
            if (!string.IsNullOrWhiteSpace(module.TheoryDocumentId)) fs.Delete(module.TheoryDocumentId);
            connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Delete(id);
            var relatedExercises = connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Find(x => x.Module.Id == module.Id);
            foreach (var exercise in relatedExercises) DeleteExercise(exercise.Id);
        }
        public void DeleteExercise(int id)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var exercise = connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).FindById(id);
            
            if (exercise is MissingTextExercise missingText) DeleteMissingTextExercise(missingText, connection);
            else throw new NotImplementedException($"Exercise type {exercise.GetType().Name} is not implemented.");
        }
        public void DeleteTag(int tagId)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var tag = connection.GetCollection<Tag>(TagRelation.COLLECTION_NAME).FindById(tagId);
            if (tag == null) throw new ArgumentNullException(nameof(tag), "Tag not found.");
            var relations = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME).DeleteMany(x => x.RelatedTag.Id == tagId);
            connection.GetCollection<Tag>(Tag.COLLECTION_NAME).Delete(tag.Id);
        }
        public void DeleteTagRelation(int relationId)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var relation = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME).FindById(relationId);
            if (relation == null) throw new ArgumentNullException(nameof(relation), "Tag relation not found.");
            connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME).Delete(relation.Id);
        }
        public void Dispose()
        {
            connection?.Dispose();
        }
        #endregion

        #region Helpers
        private double CalculatePriority(double score, DateTime lastAttempt, double lambda = 0.1)
        {
            double days = (DateTime.Now - lastAttempt).TotalDays;
            return (1 - score) * Math.Exp(-lambda * days);
        }
        private void DeleteMissingTextExercise(MissingTextExercise exercise, LiteDatabase connection)
        {
            var relatedExercises = connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME)
                .Find(x => x.Exercise.Id == exercise.Id)
                .OfType<MissingTextExerciseSubItem>().ToArray();
            foreach (var item in relatedExercises)
            {
                var tagRelations = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME)
                    .DeleteMany(x => x.ExerciseItem.Id == item.Id);
                connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Delete(item.Id);
            }
            connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Delete(exercise.Id);
        }
        private MissingTextExerciseViewModel GetMissingTextExercise(MissingTextExercise exerciseData, ModuleItemViewModel module)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var subItems = new List<MissingTextExerciseItemViewModel>();
            var items = connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME)
                .Find(x => x.Exercise.Id == exerciseData.Id)
                .OfType<MissingTextExerciseSubItem>().ToArray();
            var exercise = new MissingTextExerciseViewModel(exerciseData, module);
            foreach (var item in items)
            {
                var subItem = new MissingTextExerciseItemViewModel(item, exercise);
                var relatedTags = connection.GetCollection<TagRelation>(TagRelation.COLLECTION_NAME)
                    .Find(x => x.Exercise.Id == exerciseData.Id && x.ExerciseItem.Id == item.Id)
                    .Select(x => new AttachedTagViewModel(x.RelatedTag, x, subItem));
                subItem.Tags = new(relatedTags);
            }
            exercise.Items = new(subItems);
            return exercise;
        }
        #endregion
    }
}
