using LiteDB;

namespace ShareLingo.Core.Model.Database
{
    public class DocumentDatabase : IDisposable
    {
        #region Fields
        private readonly string filePath;
        private readonly string password;
        private LiteDatabase? connection;
        #endregion

        #region Constructors
        public DocumentDatabase(string filePath, string password)
        {
            this.filePath = filePath;
            this.password = password;
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            connection?.Dispose();
        }
        public void Open()
        {
            connection = GetDatabase();
        }
        public IEnumerable<CourseContainer> GetContainers(int skip, int limit)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            return connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Find(x => true, skip, limit).ToArray();
        }
        public IEnumerable<ModuleItem> GetModulesTo(int containerId, int skip, int limit)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            return connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME)
                .Include(x => x.Course)
                .Find(x => x.Course.Id == containerId, skip, limit);
        }
        public IEnumerable<ExerciseBase> GetExercises(int moduleId, int skip, int limit)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            return connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME)
                .Include(x => x.Module)
                .Find(x => x.Module.Id == moduleId, skip, limit);
        }
        public ResultContainer GetResultBy(int exerciseId)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            return connection.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).FindById(exerciseId);
        }
        public void AppendCourse(CourseContainer course)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME)
                .EnsureIndex(x => x.Name, unique: true); //the exception will throw if name is not unique
            var id = (int)(connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Insert(course));
            course.Id = id;
        }
        public void AppendModule(ModuleItem module)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).EnsureIndex(x => x.Name, unique: true);
            if (module.Course == null) throw new ArgumentNullException(nameof(module.Course));
            var relatedModulesHasNumber = connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME)
                .Include(x => x.Course)
                .Find(x => x.Course.Id == module.Course.Id)
                .Any(x => x.Index == module.Index);
            if (relatedModulesHasNumber)
                throw new ArgumentException($"The module with such number for container already exists ({module.Index})");
            var id = (int)(connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Insert(module));
            module.Id = id;
        }
        public void AppendExercise(ExerciseBase exercise)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            if (exercise.Module == null) throw new ArgumentNullException(nameof(exercise.Module));
            var id = (int)(connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Insert(exercise));
            exercise.Id = id;
        }
        public void AppendResult(ResultContainer result)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            if (result.Exercise == null) throw new ArgumentNullException(nameof(result.Exercise));
            if (connection.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME)
                .Include(x => x.Exercise)
                .FindOne(x => x.Exercise.Id == result.Exercise.Id) != null)
                throw new ArgumentException("The result for this exercise already exists.");
            var id = (int)(connection.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Insert(result));
            result.Id = id;
        }
        public void UpdateCourse(CourseContainer course)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME)
                .EnsureIndex(x => x.Name, unique: true); //the exception will throw if name is not unique
            var updated = connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Update(course);
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
        public void UpdateExercise(ExerciseBase exercise)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            if (exercise.Module == null) throw new ArgumentNullException(nameof(exercise.Module));
            var updated = connection.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Update(exercise);
            if (!updated) throw new InvalidOperationException("The entity wasn't updated.");
        }
        public void UpdateResult(ResultContainer result)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            if (result.Exercise == null) throw new ArgumentNullException(nameof(result.Exercise));
            var updated = connection.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Update(result);
            if (!updated) throw new InvalidOperationException("The entity wasn't updated.");
        }
        public void DeleteCourse(CourseContainer course)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var relatedModules = GetModulesTo(course.Id, 0, int.MaxValue);
            foreach (var relatedModule in relatedModules) DeleteModuleCascadeHelper(connection, relatedModule);
            connection.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Include(x => x.Course).DeleteMany(x => x.Course.Id == course.Id);
            var deleted = connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Delete(course.Id);
            if (!deleted) throw new InvalidOperationException("The entity wasn't deleted.");
        }
        public void DeleteModule(ModuleItem module)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            DeleteModuleCascadeHelper(connection, module);
        }
        public void DeleteExercise(ExerciseBase exercise)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            DeleteExerciseCascadeHelper(connection, exercise);
        }
        public void DeleteResult(ResultContainer result)
        {
            if (connection == null) throw new ArgumentException("Connection must be opened.");
            var _deleted = connection.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Delete(result.Id);
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
            var relatedExrcises = db.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME)
                .Include(x => x.Module)
                .Find(x => x.Module.Id == module.Id);
            foreach (var exercise in relatedExrcises)
                DeleteExerciseCascadeHelper(db, exercise);
            var deleted = db.GetCollection<ModuleItem>(ModuleItem.COLLECTION_NAME).Delete(module.Id);
            if (!deleted) throw new InvalidOperationException("The entity wasn't delted.");
        }
        private void DeleteExerciseCascadeHelper(LiteDatabase db, ExerciseBase exercise)
        {
            var relatedResult = db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME)
                .Include(x => x.Exercise).FindOne(x => x.Exercise.Id == exercise.Id);
            if (relatedResult != null)
            {
                var _deleted = db.GetCollection<ResultContainer>(ResultContainer.COLLECTION_NAME).Delete(relatedResult.Id);
                if (!_deleted) throw new InvalidOperationException("The entity wasn't deleted.");
            }
            var deleted = db.GetCollection<ExerciseBase>(ExerciseBase.COLLECTION_NAME).Delete(exercise.Id);
            if (!deleted) throw new InvalidOperationException("The entity wasn't deleted.");
        }
        #endregion
    }
}
