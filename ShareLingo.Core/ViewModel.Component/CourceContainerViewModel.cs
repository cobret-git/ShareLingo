using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model.Database.Component;
using System.ComponentModel;
using System.Xml.Linq;

namespace ShareLingo.Core.ViewModel.Component
{
    public partial class CourseContainerViewModel : ObservableObject, ICloneable
    {
        #region Fields
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string author = string.Empty;
        [ObservableProperty] private ushort year = default;
        [ObservableProperty] private string nativeLanguageCode = string.Empty;
        [ObservableProperty] private string foreignLanguageCode = string.Empty;
        [ObservableProperty] private string pictureCoverPath = string.Empty;
        [ObservableProperty] private string pictureCoverAbsolutePath = string.Empty;
        #endregion

        #region Constructors
        public CourseContainerViewModel() { }
        public CourseContainerViewModel(CourseContainer container, string absoluteCoverPath)
        {
            this.Id = container.Id;
            this.Name = container.Name;
            this.Description = container.Description;
            this.Author = container.Author;
            this.Year = container.Year;
            this.NativeLanguageCode = container.NativeLanguageCode;
            this.ForeignLanguageCode = container.ForeignLanguageCode;
            this.PictureCoverPath = container.PictureCoverPath;
            this.PictureCoverAbsolutePath = absoluteCoverPath;
        }
        #endregion

        #region Properties
        public int Id { get; private set; }
        #endregion

        #region Methods
        public object Clone()
        {
            return new CourseContainerViewModel()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Author = this.Author,
                Year = this.Year,
                NativeLanguageCode = this.NativeLanguageCode,
                ForeignLanguageCode = this.ForeignLanguageCode,
                PictureCoverPath = this.PictureCoverPath,
                PictureCoverAbsolutePath = this.PictureCoverAbsolutePath
            };
        }
        public void Merge(CourseContainerViewModel other)
        {
            this.Id = other.Id;
            this.Name = other.Name;
            this.Description = other.Description;
            this.Author = other.Author;
            this.Year = other.Year;
            this.NativeLanguageCode = other.NativeLanguageCode;
            this.ForeignLanguageCode = other.ForeignLanguageCode;
            this.PictureCoverPath = other.PictureCoverPath;
            this.PictureCoverAbsolutePath = other.PictureCoverAbsolutePath;
        }
        public CourseContainer ToDataItem()
        {
            return new CourseContainer()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Author = this.Author,
                Year = this.Year,
                NativeLanguageCode = this.NativeLanguageCode,
                ForeignLanguageCode = this.ForeignLanguageCode,
                PictureCoverPath = this.PictureCoverPath
            };
        }
        #endregion
    }
}
