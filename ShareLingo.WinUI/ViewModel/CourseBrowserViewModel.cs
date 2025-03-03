using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using NetForge.Core.EventArgs;
using ShareLingo.WinUI.Services;
using ShareLingo.WinUI.ViewModel.Component;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MSG = ShareLingo.WinUI.Resources.Strings.Messages;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class CourseBrowserViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IEventAggregator eventAggregator;
        private readonly IDataManager dataManager;
        private readonly IContentManager contentManager;
        #endregion

        #region Constructors
        public CourseBrowserViewModel(
            IEventAggregator eventAggregator,
            IDataManager dataManager,
            IContentManager contentManager) 
            :base(eventAggregator)
        {
            this.dataManager = dataManager;
            this.contentManager = contentManager;
        }
        #endregion

        #region Properties
        public ObservableCollection<CourseContainerViewModel> Items { get; } = new();
        #endregion

        #region Methods
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
        [RelayCommand] private async Task CreateCourse()
        {
            try
            {

                var validator = dataManager.GetCourseNameValidator();
                var prompt = await contentManager.ShowPrompt(MSG.courseBrowser_enterCourseNamePrompt, null);
                if (prompt.Result != DialogResult.Ok || !validator.IsValid(prompt.Prompt, out _)) return;
                if (dataManager.GetContaienrs(0, int.MaxValue).Any(x => x.Name == prompt.Prompt))
                    await contentManager.ShowError(MSG.courseBrowser_enteredCourseNameAlreadyInUseError);
                else
                {
                    var course = dataManager.CreateCourse(prompt.Prompt);
                    Items.Add(course);
                    contentManager.InspectCourse(course);
                }
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand] private void OpenCourse(CourseContainerViewModel? item)
        {
            try
            {
                if (item == null) return;
                contentManager.InspectCourse(item);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand] private async Task DeleteCourse(CourseContainerViewModel? item)
        {
            try
            {
                if (item == null) return;
                var confirm = await contentManager.ShowConfirm(MSG.courseBrowser_deleteSelectedCourseConfirm, DialogButtons.YesNo);
                if (confirm.Result != DialogResult.Yes)
                {
                    dataManager.DeleteCourse(item);
                    Items.Remove(item);
                }
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand] private void OnLoaded()
        {
            try
            {
                Items.Clear();
                var items = dataManager.GetContaienrs(0, int.MaxValue);
                foreach (var item in items) Items.Add(item);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        #endregion

        #region Handlers
        protected override void OnPageClosing(PageClosingEventArgs e)
        {
            throw new NotImplementedException();
        }
        protected override void OnPageClosed(PageClosedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
