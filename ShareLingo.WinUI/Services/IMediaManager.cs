using ShareLingo.WinUI.ViewModel.Component;
using System;
using System.IO.Abstractions;

namespace ShareLingo.WinUI.Services
{
    public interface IMediaManager
    {
        MediaData ImportCourseImage(string filePath);
        MediaData GetCourseImageData(string relativePath);
        MediaData ImportModuleImage(string filePath);
        MediaData GetModuleImage(string relativePath);
    }
    public class MediaManager : IMediaManager
    {
        #region Fields
        private readonly IBuildInfoManager bldMng;
        private readonly IFileSystem fileSystem;
        private readonly string imageDirectory;
        #endregion

        #region Constructors
        public MediaManager(IBuildInfoManager bldMng)
        {
            this.bldMng = bldMng;
            this.fileSystem = bldMng.FileSystem;

            imageDirectory = fileSystem.Path.Combine(bldMng.MediaDirectory, "Images");

            CreateImageDirectoryIfNotExists();
        }
        #endregion

        #region Methods
        public MediaData ImportCourseImage(string filePath)
        {
            var fileName = GetNewImageName(fileSystem.Path.GetExtension(filePath));
            fileSystem.File.Copy(filePath, fileSystem.Path.Combine(imageDirectory, fileName));

            return new MediaData()
            {
                AbsolutePath = fileSystem.Path.Combine(imageDirectory, fileName),
                RelativePath = fileSystem.Path.Combine("Images", fileName)
            };
        }
        public MediaData GetCourseImageData(string relativePath)
        {
            return new MediaData()
            {
                AbsolutePath = fileSystem.Path.Combine(bldMng.MediaDirectory, relativePath),
                RelativePath = relativePath
            };
        }
        public MediaData ImportModuleImage(string filePath)
        {
            var fileName = GetNewImageName(fileSystem.Path.GetExtension(filePath));
            fileSystem.File.Copy(filePath, fileSystem.Path.Combine(imageDirectory, fileName));

            return new MediaData()
            {
                AbsolutePath = fileSystem.Path.Combine(imageDirectory, fileName),
                RelativePath = fileSystem.Path.Combine("Images", fileName)
            };
        }
        public MediaData GetModuleImage(string relativePath)
        {
            return new MediaData()
            {
                AbsolutePath = fileSystem.Path.Combine(bldMng.MediaDirectory, relativePath),
                RelativePath = relativePath
            };
        }
        #endregion

        #region Helpers
        private void CreateImageDirectoryIfNotExists()
        {
            if (!fileSystem.Directory.Exists(imageDirectory)) fileSystem.Directory.CreateDirectory(imageDirectory);
        }
        private string GetNewImageName(string fileExtension)
        {
            var fileName = $"{Guid.NewGuid().ToString().Substring(0, 6)}{fileExtension}";
            while (fileSystem.File.Exists(fileSystem.Path.Combine(imageDirectory, fileName)))
                fileName = $"{Guid.NewGuid().ToString().Substring(0, 6)}{fileExtension}";
            return fileName;
        }
        #endregion
    }
}
