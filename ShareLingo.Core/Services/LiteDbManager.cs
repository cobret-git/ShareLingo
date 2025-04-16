using LiteDB;
using ShareLingo.Core.Model;
using ShareLingo.Core.ViewModel;
using System.IO.Abstractions;

namespace ShareLingo.Core.Services
{
    public class LiteDbManager
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly string filePath;
        private readonly string password;
        private LiteDatabase? connection;
        #endregion

        #region Methods
        public ExerciseViewModelBase[] GetExercisesData(ModuleItemViewModel module)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (module == null) throw new ArgumentException("Module must be set.");
            var dataItems = connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Find(x => x.Module.Id == module.Item.Id);
            var result = new List<ExerciseViewModelBase>();
            foreach (var item in dataItems)
            {
                ExerciseViewModelBase exercise;
                switch (item.Type)
                {
                    case ExerciseType.MissingText:
                        exercise = new MissingTextExerciseViewModel(item as MissingTextExercise);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                result.Add(exercise);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Saves the exercise data to the database.
        /// </summary>
        /// <param name="exercise"></param>
        /// <exception cref="ArgumentException"></exception>
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
                    connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Delete(item.Id);
            }    
            else
                exercise.Item.Id = (int)(connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Insert(exercise.Item));

            foreach(var item in exercise.Items)
            {
                if (item.Item.Id != default) connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Update(item.Item);
                else item.Item.Id = (int)(connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Insert(item.Item));
            }
        }

        /// <summary>
        /// Retreives the module data from the database related to the <paramref name="course"/>. The data is paginated using <paramref name="skip"/> and <paramref name="limit"/> parameters.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ModuleItemViewModel[] GetModulesData(CourseContainerViewModel course, int skip, int limit)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            else if (course == null) throw new ArgumentException("Course must be set.");

            var dataItems = connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Find(x => x.Course.Id == course.Item.Id).Skip(skip).Take(limit);
            var fs = connection.GetStorage<string>();
            var result = new List<ModuleItemViewModel>();
            foreach (var item in dataItems)
            {
                var module = new ModuleItemViewModel(item);
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

        /// <summary>
        /// Saves the module data to the database. Ensures that the module's index is unique within the course.
        /// </summary>
        /// <param name="item">The module that needs to be saved.</param>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Retreives the course data from the database. The data is paginated using <paramref name="skip"/> and <paramref name="limit"/> parameters.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Saves the course data to the database. Ensures that the course's name is unique.s
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Deletes the course data from the database. Deletes all related modules and their data.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
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
        }

        /// <summary>
        /// Deletes the module data from the database. Deletes all related exercises and their data.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>
        /// Deletes the exercise data from the database. Deletes all related subitems and their data.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        public void DeleteExercise(int id)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var exercise = connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).FindById(id);
            
            if (exercise is MissingTextExercise missingText) DeleteMissingTextExercise(missingText, connection);
        }
        #endregion

        #region Helpers
        private void DeleteMissingTextExercise(MissingTextExercise exercise, LiteDatabase connection)
        {
            var relatedExercises = connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME)
                .Find(x => x.Exercise.Id == exercise.Id)
                .OfType<MissingTextExerciseSubItem>().ToArray();
            foreach (var item in relatedExercises)
                connection.GetCollection<ExerciseSubItemBase>(ExerciseSubItemBase.COLLECTION_NAME).Delete(item.Id);
            connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Delete(exercise.Id);
        }
        #endregion
    }
}
