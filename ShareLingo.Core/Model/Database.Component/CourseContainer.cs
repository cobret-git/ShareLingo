using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using NetForge.Core;

namespace ShareLingo.Core.Model
{
    public class CourseContainer : ObservableObject, ICloneable<CourseContainer>
    {
        #region Consts
        public const string COLLECTION_NAME = "CourseContaienrs";
        #endregion

        #region Fields
        private string name = string.Empty;
        private string author = string.Empty;
        private ushort year;
        private string nativeLanguageCode = string.Empty;
        private string foreignLanguageCode = string.Empty;
        private string coverId = string.Empty;
        private string docId = string.Empty;
        #endregion

        #region Properties
        [BsonId] public int Id { get; set; }
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        public string Author { get => author; set { author = value; OnPropertyChanged(); } }
        public ushort Year { get => year; set { year = value; OnPropertyChanged(); } }
        public string NativeLanguageCode { get => nativeLanguageCode; set { nativeLanguageCode = value; OnPropertyChanged(); } }
        public string ForeignLanguageCode { get => foreignLanguageCode; set { foreignLanguageCode = value; OnPropertyChanged(); } }
        public string CoverId { get => coverId; set { coverId = value; OnPropertyChanged(); } }
        public string DescriptionDocumentId { get => docId; set { docId = value; OnPropertyChanged(); } }
        #endregion

        #region Methods
        public CourseContainer Clone()
        {
            return new CourseContainer()
            {
                Id = Id,
                Name = Name,
                Author = Author,
                Year = Year,
                NativeLanguageCode = NativeLanguageCode,
                ForeignLanguageCode = ForeignLanguageCode,
                CoverId = CoverId,
                DescriptionDocumentId = DescriptionDocumentId
            };
        }
        #endregion
    }
}
