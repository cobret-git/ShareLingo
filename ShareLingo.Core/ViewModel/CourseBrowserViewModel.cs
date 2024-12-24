using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareLingo.Core.Services;
using ShareLingo.Core.ViewModel.Component;
using System.Collections.ObjectModel;
using msg = ShareLingo.Core.Resources.Messages;

namespace ShareLingo.Core.ViewModel
{
    public partial class CourseBrowserViewModel : ObservableObject
    {
        #region Fields
        private readonly ILoggerManager loggerManager;
        private readonly IDataManager dataManager;
        private readonly IContentManager contentManager;
        #endregion

        #region Constructors
        public CourseBrowserViewModel(ILoggerManager loggerManager, 
            IDataManager dataManager,
            IContentManager contentManager)
        {
            this.loggerManager = loggerManager;
            this.dataManager = dataManager;
            this.contentManager = contentManager;
        }
        #endregion

        #region Properties
        public ObservableCollection<CourseContainerViewModel> Items { get; } = new();
        #endregion

        #region Methods
        [RelayCommand] private async Task CreateCourse()
        {
            try
            {

                var validator = dataManager.GetCourseNameValidator();
                var prompt = await contentManager.ShowPrompt(msg.courseBrowser_enterCourseNamePrompt, null);
                if (prompt.Result != DialogResult.Ok || !validator.IsValid(prompt.Prompt)) return;
                if (dataManager.GetContaienrs(0, int.MaxValue).Any(x => x.Name == prompt.Prompt))
                    await contentManager.ShowError(msg.courseBrowser_enteredCourseNameAlreadyInUseError);
                else
                {
                    var course = dataManager.CreateCourse(prompt.Prompt);
                    Items.Add(course);
                    contentManager.InspectCourse(course);
                }
            }
            catch (Exception ex) { loggerManager.Debug(ex); }
        }
        [RelayCommand] private void OpenCourse(CourseContainerViewModel? item)
        {
            try
            {
                if (item == null) return;
                contentManager.InspectCourse(item);
            }
            catch (Exception ex) { loggerManager.Debug(ex); }
        }
        [RelayCommand] private async Task DeleteCourse(CourseContainerViewModel? item)
        {
            try
            {
                if (item == null) return;
                var confirm = await contentManager.ShowConfirm(msg.courseBrowser_deleteSelectedCourseConfirm, DialogButtons.YesNo);
                if (confirm.Result != DialogResult.Yes)
                {
                    dataManager.DeleteCourse(item);
                    Items.Remove(item);
                }
            }
            catch (Exception ex) { loggerManager.Debug(ex); }
        }
        [RelayCommand] private void OnLoaded()
        {
            try
            {
                Items.Clear();
                var items = dataManager.GetContaienrs(0, int.MaxValue);
                foreach (var item in items) Items.Add(item);
            }
            catch (Exception ex) { loggerManager.Debug(ex); }
        }
        #endregion
    }
}
