using LiteDB;

namespace ShareLingo.Core.Model.Database.Component
{
    public class ModuleItem
    {
        #region Consts
        public const string COLLECTION_NAME = "ModuleItems";
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        /// <summary>
        /// The unique module's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        [BsonRef(CourseContainer.COLLECTION_NAME)] public CourseContainer Course { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        /// <summary>
        /// The unique module's number.
        /// </summary>
        public int ModuleNumber { get; set; }
        public string PictureCoverPath { get; set; } = string.Empty;
        public string TheoryDocumentPath { get; set;} = string.Empty;
        #endregion
    }
}
