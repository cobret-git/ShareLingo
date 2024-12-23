using ShareLingo.Core.Model.Database;
using ShareLingo.Core.ViewModel.Component;
using System.IO.Abstractions;

namespace ShareLingo.Core.Services
{
    public interface IDataManager
    {
        IEnumerable<CourseContainerViewModel> GetContaienrs(int skip, int limit);
    }
    public class DataManager : IDataManager
    {
        #region Fields
        private readonly IFileSystem fileSystem;
        private readonly IBuildInfoManager buildManager;
        private readonly DocumentDatabase db;
        #endregion

        #region Methods
        public IEnumerable<CourseContainerViewModel> GetContaienrs(int skip, int limit)
        {
            var dataItems = db.GetContainers(skip, limit);
            foreach (var item in dataItems)
            {
                var coverFilePath = fileSystem.Path.Combine(buildManager.MediaDirectoryPath, item.PictureCoverPath);
                yield return new CourseContainerViewModel(item, coverFilePath);
            }
        }
        #endregion
    }
}
