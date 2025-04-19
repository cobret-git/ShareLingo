using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace ShareLingo.Core.Model
{
    public class Tag : ObservableObject
    {
        #region Consts
        public const string COLLECTION_NAME = "Tags";
        #endregion

        #region Fields
        private string name = string.Empty;
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        [BsonRef(CourseContainer.COLLECTION_NAME)] public CourseContainer Course { get; set; } = null!;
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        #endregion
    }
}
