using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using System;
using System.Threading.Tasks;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class CourseEditorViewModel : ObservableObject
    {
        #region Properties
        public CourseContainerViewModel Container { get; }
        #endregion

        #region Methods
        [RelayCommand]
        private async Task ChangeCourseCover()
        {
            try
            {
                var openDlg = await contentManager.OpenFile(FPR.image_dlgFilter);
                if (!openDlg.FileSelected) return;
                var data = dataManager.Media.ImportCourseImage(openDlg.FileName);
                Course.PictureCoverPath = data.RelativePath;
                Course.PictureCoverAbsolutePath = data.AbsolutePath;
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        #endregion
    }
}
