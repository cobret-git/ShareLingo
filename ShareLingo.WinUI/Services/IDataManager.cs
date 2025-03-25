using ShareLingo.WinUI.Model.Database;
using ShareLingo.WinUI.Model.Database.Component;
using ShareLingo.WinUI.ViewModel;
using ShareLingo.WinUI.ViewModel.Component;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace ShareLingo.WinUI.Services
{
    public interface IDataManager : IDisposable
    {
        #region Methods
        public IMediaManager Media { get; }
        #endregion

        #region Methods
        IEnumerable<CourseContainerViewModel> GetContaienrs(int skip, int limit);
        CourseNameValidator GetCourseNameValidator();
        CourseContainerViewModel CreateCourse(string name);
        ModuleItemViewModel CreateModule(string name);
        void DeleteCourse(CourseContainerViewModel course);
        void DeleteModule(ModuleItemViewModel module);
        void SaveCourse(CourseContainerViewModel course);
        #endregion
    }
    public class DataManager : IDataManager
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly IBuildInfoManager buildManager;
        private readonly DocumentDatabase db;
        #endregion

        #region Constructors
        public DataManager(IBuildInfoManager buildManager, IMediaManager mediaManager)
        {
            this.buildManager = buildManager;
            this.fileSystem = buildManager.FileSystem;

            Media = mediaManager;

            var databaseFilePath = fileSystem.Path.Combine(buildManager.DataDirectory, "data.db");
            db = new DocumentDatabase(databaseFilePath, "nopassword");
            db.Open();
            
        }
        #endregion

        #region Properties
        public IMediaManager Media { get; }
        #endregion

        #region Methods
        public void Dispose()
        {
            db.Dispose();
        }
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
                var coverFilePath = string.IsNullOrWhiteSpace(item.PictureCoverPath)
                    ? null
                    : fileSystem.Path.Combine(buildManager.MediaDirectory, item.PictureCoverPath);
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
            return null!;
        }
        public void DeleteCourse(CourseContainerViewModel course)
        {
            db.DeleteCourse(course.ToDataItem());
        }
        public void DeleteModule(ModuleItemViewModel module)
        {
            //db.DeleteModule();
        }
        public void SaveCourse(CourseContainerViewModel course)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
