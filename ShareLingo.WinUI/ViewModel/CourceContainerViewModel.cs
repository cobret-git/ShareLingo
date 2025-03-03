using CommunityToolkit.Mvvm.ComponentModel;
using NetForge.Core;
using ShareLingo.WinUI.Model.Database.Component;
using System.ComponentModel;
using System.Xml.Linq;

namespace ShareLingo.WinUI.ViewModel.Component
{
    public partial class CourseContainerViewModel : ObservableObject, ICloneable<CourseContainerViewModel>
    {
        #region Fields
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
        [ObservableProperty] public partial string Name { get; set; } = string.Empty;
        [ObservableProperty] public partial string Description { get; set; } = string.Empty;
        [ObservableProperty] public partial string Author { get; set; } = string.Empty;
        [ObservableProperty] public partial ushort Year { get; set; }
        [ObservableProperty] public partial string NativeLanguageCode { get; set; } = string.Empty;
        [ObservableProperty] public partial string ForeignLanguageCode { get; set; } = string.Empty;
        [ObservableProperty] public partial string PictureCoverPath { get; set; } = string.Empty;
        [ObservableProperty] public partial string PictureCoverAbsolutePath { get; set; } = string.Empty;
        #endregion

        #region Methods
        public CourseContainerViewModel Clone()
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
