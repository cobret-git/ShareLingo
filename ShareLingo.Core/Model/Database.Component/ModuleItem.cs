using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using NetForge.Core;

namespace ShareLingo.Core.Model
{
    public class ModuleItem : ObservableObject, ICloneable<ModuleItem>, IMergable<ModuleItem>
    {
        #region Consts
        public const string COLLECTION_NAME = "ModuleItems";
        #endregion

        #region Fields
        private string name = string.Empty;
        private int index;
        private string coverId = string.Empty;
        private string theoryDocumentId = string.Empty;
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }

        /// <summary>
        /// The unique module's name.
        /// </summary>
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }

        /// <summary>
        /// The parent course of the module.
        /// </summary>
        [BsonRef(CourseContainer.COLLECTION_NAME)] public CourseContainer Course { get; set; } = null!;

        /// <summary>
        /// The unique module's number.
        /// </summary>
        public int Index { get => index; set { index = value; OnPropertyChanged(); } }

        /// <summary>
        /// The file id of module's cover.
        /// </summary>
        public string CoverId { get => coverId; set { coverId = value; OnPropertyChanged(); } }

        /// <summary>
        /// The file id of module's theory document.
        /// </summary>
        public string TheoryDocumentId { get => theoryDocumentId; set { theoryDocumentId = value; OnPropertyChanged(); } }
        #endregion

        #region Methods
        public ModuleItem Clone()
        {
            return new ModuleItem()
            {
                Id = Id,
                Name = Name,
                Index = Index,
                Course = Course,
                CoverId = CoverId,
                TheoryDocumentId = TheoryDocumentId
            };
        }
        public void Merge(ModuleItem other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            Id = other.Id;
            Name = other.Name;
            Index = other.Index;
            Course = other.Course;
            CoverId = other.CoverId;
            TheoryDocumentId = other.TheoryDocumentId;
        }
        #endregion
    }
}
