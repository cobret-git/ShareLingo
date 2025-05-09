using ShareLingo.Core.Model;
using ShareLingo.Core.Model.Database;
using ShareLingo.Core.ViewModel;
using ShareLingo.Core.ViewModel.Component;
using System.IO.Abstractions;

namespace ShareLingo.Core.Services
{
    /// <summary>
    /// Interface for managing data operations such as saving, deleting, and retrieving course, module, exercise, and tag data.
    /// </summary>
    public interface IDataManager : IDisposable
    {
        #region General Methods

        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        void Open();
        #endregion

        #region Get Methods
        /// <summary>
        /// Retreives the course data from the database. The data is paginated using <paramref name="skip"/> and <paramref name="limit"/> parameters.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        CourseContainerViewModel[] GetCourseData(int skip, int limit);

        /// <summary>
        /// Retreives the module data from the database related to the <paramref name="course"/>. The data is paginated using <paramref name="skip"/> and <paramref name="limit"/> parameters.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        ModuleItemViewModel[] GetModulesData(CourseContainerViewModel course, int skip, int limit);

        /// <summary>
        /// Retreives the exercise data from the database related to the <paramref name="module"/>.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        ExerciseViewModelBase[] GetExercisesData(ModuleItemViewModel module);

        ExerciseSubItemViewModelBase[] GetItemsToRepeat(CourseContainerViewModel course, int limit);

        /// <summary>
        /// Retreives the tag data from the database related to the <paramref name="subItem"/>. If relation does not exists, it creates a new one.
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="subItem"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        AttachedTagViewModel? GetTag(string? tagName, ExerciseSubItemViewModelBase subItem);
        #endregion

        #region Save Methods

        /// <summary>
        /// Saves the course data to the database. Ensures that the course's name is unique.s
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentException"></exception>
        void SaveCourseData(CourseContainerViewModel item);

        /// <summary>
        /// Saves the module data to the database. Ensures that the module's index is unique within the course.
        /// </summary>
        /// <param name="item">The module that needs to be saved.</param>
        /// <exception cref="ArgumentException"></exception>
        void SaveModuleData(ModuleItemViewModel item);

        /// <summary>
        /// Saves the exercise data to the database. Ensures that the exercise's name is unique within the module. 
        /// </summary>
        /// <param name="exercise"></param>
        /// <exception cref="ArgumentException"></exception>
        void SaveExercise(MissingTextExerciseViewModel exercise);


        /// <summary>
        /// Saves the exercise progress to the database.
        /// </summary>
        /// <param name="exercise"></param>
        /// <exception cref="ArgumentException"></exception>
        void SaveExerciseProgress(ExerciseViewModelBase exercise);

        /// <summary>
        /// Saves the tag data to the database. Ensures that the tag's name is unique within the course and module.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        void SaveTag(AttachedTagViewModel item);
        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes the course data from the database. Deletes all related modules and their data.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        void DeleteCourseData(int id);

        /// <summary>
        /// Deletes the module data from the database. Deletes all related exercises and their data.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        void DeleteModuleData(int id);

        /// <summary>
        /// Deletes the exercise data from the database. Deletes all related subitems and their data.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        void DeleteExercise(int id);

        /// <summary>
        /// Deletes the tag data from the database. Deletes all related tag relations and their data.
        /// </summary>
        /// <param name="tagId"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        void DeleteTag(int tagId);

        /// <summary>
        /// Deletes the tag relation data from the database.
        /// </summary>
        /// <param name="relationId"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        void DeleteTagRelation(int relationId);
        #endregion
    }
}
