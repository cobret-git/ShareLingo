using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using System;

namespace ShareLingo.Core.Model
{
    public class ExerciseContainer : ObservableObject
    {
        #region Consts
        public const string COLLECTION_NAME = "ExerciseContainers";
        #endregion

        #region Fields
        private ExerciseType type;
        private string description = string.Empty;
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        [BsonRef(ModuleItem.COLLECTION_NAME)] public ModuleItem Module { get; set; } = null!;
        public ExerciseType Type { get => type; set { type = value; OnPropertyChanged(); } }
        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public string[] AllAudioPaths { get; set; } = Array.Empty<string>();
        public string[] AllVideoPaths { get; set; } = Array.Empty<string>();
        public string[] AllImagePaths { get; set; } = Array.Empty<string>();
        public string[] AllDocumentsPaths { get; set; } = Array.Empty<string>();
        public ExerciseContentBase Content { get; set; } = null!;
        #endregion
    }
}
