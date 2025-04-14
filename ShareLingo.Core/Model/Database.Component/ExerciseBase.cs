using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using System;

namespace ShareLingo.Core.Model
{
    public abstract class ExerciseBase : ObservableObject
    {
        #region Consts
        public const string COLLECTION_NAME = "Exercises";
        #endregion

        #region Fields
        private string name = string.Empty;
        private string description = string.Empty;
        private bool completed = false;
        private DateTime completedDate = default;
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        [BsonRef(ModuleItem.COLLECTION_NAME)] public ModuleItem Module { get; set; } = null!;
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public bool Completed { get => completed; set { completed = value; OnPropertyChanged(); } }
        public DateTime CompletedDate { get => completedDate; set { completedDate = value; OnPropertyChanged(); } }
        #endregion
    }
}
