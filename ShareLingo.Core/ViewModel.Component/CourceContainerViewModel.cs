using CommunityToolkit.Mvvm.ComponentModel;
using ShareLingo.Core.Model.Database.Component;

namespace ShareLingo.Core.ViewModel.Component
{
    public partial class CourseContainerViewModel : ObservableObject
    {
        #region Fields
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string author = string.Empty;
        [ObservableProperty] private ushort year = default;
        [ObservableProperty] private string nativeLanguageCode = string.Empty;
        [ObservableProperty] private string foreignLanguageCode = string.Empty;
        [ObservableProperty] private string pictureCoverAbsolutePath = string.Empty;
        #endregion

        #region Constructors
        public CourseContainerViewModel(CourseContainer container, string absoluteCoverPath)
        {
            this.Name = container.Name;
            this.Description = container.Description;
            this.Author = container.Author;
            this.Year = container.Year;
            this.NativeLanguageCode = container.NativeLanguageCode;
            this.ForeignLanguageCode = container.ForeignLanguageCode;
            this.PictureCoverAbsolutePath = absoluteCoverPath;
        }
        #endregion
    }
}
