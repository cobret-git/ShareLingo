using LiteDB;

namespace ShareLingo.WinUI.Model.Database.Component
{
    public class ExerciseContainer
    {
        #region Consts
        public const string COLLECTION_NAME = "ExerciseContainers";
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        [BsonRef(ModuleItem.COLLECTION_NAME)] public ModuleItem Module { get; set; } = null!;
        public ExerciseType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string[] AllAudioPaths { get; set; } = Array.Empty<string>();
        public string[] AllVideoPaths { get; set; } = Array.Empty<string>();
        public string[] AllImagePaths { get; set; } = Array.Empty<string>();
        public string[] AllDocumentsPaths { get; set; } = Array.Empty<string>();
        public ExerciseContentBase Content { get; set; } = null!;
        #endregion
    }
}
