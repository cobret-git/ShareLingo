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
        //public void DeleteCourseData(CourseContainerViewModel item)
        //{
        //    if (connection == null) throw new ArgumentException("Connection must be opened.");
        //    var fs = connection.GetStorage<string>();
        //    if (!string.IsNullOrWhiteSpace(item.Item.DescriptionDocumentId))
        //        fs.Delete(item.Item.DescriptionDocumentId);
        //    if (!string.IsNullOrWhiteSpace(item.Item.CoverId))
        //        fs.Delete(item.Item.CoverId);
        //    connection.GetCollection<CourseContainer>(CourseContainer.COLLECTION_NAME).Delete(item.Item.Id);
        //    item.Dispose();
        //}
        #endregion
    }
}
