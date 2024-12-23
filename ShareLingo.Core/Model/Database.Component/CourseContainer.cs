using LiteDB;

namespace ShareLingo.Core.Model.Database.Component
{
    public class CourseContainer
    {
        #region Consts
        public const string COLLECTION_NAME = "CourseContaienrs";
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public ushort Year { get; set; }
        public string NativeLanguageCode { get; set; } = string.Empty;
        public string ForeignLanguageCode { get; set; } = string.Empty;
        public string PictureCoverPath { get; set; } = string.Empty;
        #endregion
    }
}
