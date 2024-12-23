using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareLingo.Core.Services;
using ShareLingo.Core.ViewModel.Component;
using System.Collections.ObjectModel;

namespace ShareLingo.Core.ViewModel
{
    public partial class CourseBrowserViewModel : ObservableObject
    {
        #region Fields
        private readonly ILoggerManager loggerManager;
        private readonly IDataManager dataManager;
        private readonly IContentManager contentManager;
        #endregion

        #region Properties
        public ObservableCollection<CourseContainerViewModel> Items { get; } = new();
        #endregion

        #region Methods
        [RelayCommand] private async Task CreateCourse()
        {
            try
            {
                var confirm = 
            }
            catch (Exception ex) { loggerManager.Debug(ex); }
        }
        [RelayCommand] private void OpenCourse(CourseContainerViewModel? item)
        {
            try
            {

            }
            catch (Exception ex) { loggerManager.Debug(ex); }
        }
        [RelayCommand] private void DeleteCourse(CourseContainerViewModel? item)
        {
            try
            {

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
