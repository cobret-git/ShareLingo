using ShareLingo.Core.Model.Database;
using ShareLingo.Core.Model.Database.Component;
using ShareLingo.Core.ViewModel.Component;
using System.IO.Abstractions;

namespace ShareLingo.Core.Services
{
    public interface IDataManager
    {
        #region Methods
        public IMediaManager Media { get; }
        #endregion

        IEnumerable<CourseContainerViewModel> GetContaienrs(int skip, int limit);
        CourseNameValidator GetCourseNameValidator();
        CourseContainerViewModel CreateCourse(string name);
        ModuleItemViewModel CreateModule(string name);
        void DeleteCourse(CourseContainerViewModel course);
        void DeleteModule(ModuleItemViewModel module);
        void SaveCourse(CourseContainerViewModel course);
    }
    public class DataManager : IDataManager
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly IBuildInfoManager buildManager;
        private readonly DocumentDatabase db;
        #endregion

        #region Methods
        public CourseNameValidator GetCourseNameValidator()
        {
            var chars = fileSystem.Path.GetInvalidFileNameChars();
            return new CourseNameValidator(chars);
        }
        public IEnumerable<CourseContainerViewModel> GetContaienrs(int skip, int limit)
        {
            var dataItems = db.GetContainers(skip, limit);
            foreach (var item in dataItems)
            {
                var coverFilePath = fileSystem.Path.Combine(buildManager.MediaDirectoryPath, item.PictureCoverPath);
                yield return new CourseContainerViewModel(item, coverFilePath);
            }
        }
        public CourseContainerViewModel CreateCourse(string name)
        {
            var courseContainer = new CourseContainer() 
            { 
                Name = name,
                Year = (ushort)DateTime.Today.Year
            };
            db.AppendCourse(courseContainer);
            return new CourseContainerViewModel(courseContainer, string.Empty);
        }
        public ModuleItemViewModel CreateModule(string name)
        {

        }
        public void DeleteCourse(CourseContainerViewModel course)
        {
            db.DeleteCourse(course.ToDataItem());
        }
        public void DeleteModule(ModuleItemViewModel module)
        {
            db.DeleteModule();
        }
        #endregion
    }
}
