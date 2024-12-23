using LiteDB;
using ShareLingo.Core.Model.Database.Component;

namespace ShareLingo.Core.Model.Database
{
    public class DocumentDatabase
    {
        #region Fields
        private readonly string filePath;
        private readonly string password;
        #endregion

        #region Constructors
        public DocumentDatabase(string filePath, string password)
        {
            this.filePath = filePath;
            this.password = password;
        }
        #endregion

        #region Methods
        public IEnumerable<CourseContainer> GetContainers(int skip, int limit)
        {
            using var db = GetDatabase();
            return db.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Find(x => true, skip, limit);
        }
        public IEnumerable<ModuleItem> GetModulesTo(int containerId, int skip, int limit)
        {
            using var db = GetDatabase();
            return db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME)
                .Include(x => x.Course)
                .Find(x => x.Course.Id == containerId, skip, limit);
        }
        public IEnumerable<ExerciseContainer> GetExercises(int moduleId, int skip, int limit)
        {
            using var db = GetDatabase();
            return db.GetCollection<ExerciseContainer>(ExerciseContainer.COLLECTION_NAME)
                .Include(x => x.Module)
                .Find(x => x.Module.Id == moduleId, skip, limit);
        }
        public ResultContainer GetResultBy(int exerciseId)
        {
            using var db = GetDatabase();
            return db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).FindById(exerciseId);
        }
        public void AppendCourse(CourseContainer course)
        {
            using var db = GetDatabase();
            db.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME)
                .EnsureIndex(x => x.Name, unique: true); //the exception will throw if name is not unique
            var id = (int)(db.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Insert(course));
            course.Id = id;
        }
        public void AppendModule(ModuleItem module)
        {
            using var db = GetDatabase();
            db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).EnsureIndex(x => x.Name, unique: true);
            if (module.Course == null) throw new ArgumentNullException(nameof(module.Course));
            var relatedModulesHasNumber = db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME)
                .Include(x => x.Course)
                .Find(x => x.Course.Id == module.Course.Id)
                .Any(x => x.ModuleNumber == module.ModuleNumber);
            if (relatedModulesHasNumber)
                throw new ArgumentException($"The module with such number for container already exists ({module.ModuleNumber})");
            var id = (int)(db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Insert(module));
            module.Id = id;
        }
        public void AppendExercise(ExerciseContainer exercise)
        {
            using var db = GetDatabase();
            if (exercise.Module == null) throw new ArgumentNullException(nameof(exercise.Module));
            var id = (int)(db.GetCollection<ExerciseContainer>(ExerciseContainer.COLLECTION_NAME).Insert(exercise));
            exercise.Id = id;
        }
        public void AppendResult(ResultContainer result)
        {
            using var db = GetDatabase();
            if (result.Exercise == null) throw new ArgumentNullException(nameof(result.Exercise));
            if (db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME)
                .Include(x => x.Exercise)
                .FindOne(x => x.Exercise.Id == result.Exercise.Id) != null)
                throw new ArgumentException("The result for this exercise already exists.");
            var id = (int)(db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Insert(result));
            result.Id = id;
        }
        public void UpdateCourse(CourseContainer course)
        {
            using var db = GetDatabase();
            db.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME)
                .EnsureIndex(x => x.Name, unique: true); //the exception will throw if name is not unique
            var updated = db.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Update(course);
            if (!updated) throw new InvalidOperationException("The entity wasn't updated.");
        }
        public void UpdateModule(ModuleItem module)
        {
            using var db = GetDatabase();

            db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).EnsureIndex(x => x.Name, unique: true);
            if (module.Course == null) throw new ArgumentNullException(nameof(module.Course));
            //var relatedModulesHasNumber = db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME)
            //    .Include(x => x.Course)
            //    .Find(x => x.Course.Id == module.Course.Id)
            //    .Any(x => x.ModuleNumber == module.ModuleNumber);
            //if (relatedModulesHasNumber)
            //    throw new ArgumentException($"The module with such number for container already exists ({module.ModuleNumber})");
            var updated = db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Update(module);
            if (!updated) throw new InvalidOperationException("The enity wasn't updated.");
        }
        public void UpdateExercise(ExerciseContainer exercise)
        {
            using var db = GetDatabase();
            if (exercise.Module == null) throw new ArgumentNullException(nameof(exercise.Module));
            var updated = db.GetCollection<ExerciseContainer>(ExerciseContainer.COLLECTION_NAME).Update(exercise);
            if (!updated) throw new InvalidOperationException("The entity wasn't updated.");
        }
        public void UpdateResult(ResultContainer result)
        {
            using var db = GetDatabase();
            if (result.Exercise == null) throw new ArgumentNullException(nameof(result.Exercise));
            var updated = db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Update(result);
            if (!updated) throw new InvalidOperationException("The entity wasn't updated.");
        }
        public void DeleteCourse(CourseContainer course)
        {
            using var db = GetDatabase();
            var relatedModules = GetModulesTo(course.Id, 0, int.MaxValue);
            foreach(var relatedModule in relatedModules) DeleteModuleCascadeHelper(db, relatedModule);
            db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Include(x => x.Course).DeleteMany(x => x.Course.Id == course.Id);
            var deleted = db.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Delete(course.Id);
            if (!deleted) throw new InvalidOperationException("The entity wasn't deleted.");
        }
        public void DeleteModule(ModuleItem module)
        {
            using var db = GetDatabase();
            DeleteModuleCascadeHelper(db, module);
        }
        public void DeleteExercise(ExerciseContainer exercise)
        {
            using var db = GetDatabase();
            DeleteExerciseCascadeHelper(db, exercise);
        }
        public void DeleteResult(ResultContainer result)
        {
            using var db = GetDatabase();
            var _deleted = db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Delete(result.Id);
            if (!_deleted) throw new InvalidOperationException("The entity wasn't deleted.");
        }
        #endregion

        #region Helpers
        private LiteDatabase GetDatabase()
        {
            return new LiteDatabase($"FileName={filePath};Password={password}");
        }
        private void DeleteModuleCascadeHelper(LiteDatabase db, ModuleItem module)
        {
            var relatedExrcises = db.GetCollection<ExerciseContainer>(ExerciseContainer.COLLECTION_NAME)
                .Include(x => x.Module)
                .Find(x => x.Module.Id == module.Id);
            foreach (var exercise in relatedExrcises)
                DeleteExerciseCascadeHelper(db, exercise);
            var deleted = db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Delete(module.Id);
            if (!deleted) throw new InvalidOperationException("The entity wasn't delted.");
        }
        private void DeleteExerciseCascadeHelper(LiteDatabase db, ExerciseContainer exercise)
        {
            var relatedResult = db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME)
                .Include(x => x.Exercise).FindOne(x => x.Exercise.Id == exercise.Id);
            if (relatedResult != null)
            {
                var _deleted = db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Delete(relatedResult.Id);
                if (!_deleted) throw new InvalidOperationException("The entity wasn't deleted.");
            }
            var deleted = db.GetCollection<ExerciseContainer>(ExerciseContainer.COLLECTION_NAME).Delete(exercise.Id);
            if (!deleted) throw new InvalidOperationException("The entity wasn't deleted.");
        }
        #endregion
    }
}
