using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using ShareLingo.Core.Model;
using ShareLingo.Core.Services;
using System.Collections.ObjectModel;

namespace ShareLingo.Core.ViewModel
{
    public partial class CourseViewerViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IDataManager dataManager;
        #endregion

        #region Constructors
        public CourseViewerViewModel(IEventAggregator eventAggregator, IDataManager dataManager) 
            : base(eventAggregator)
        {
            this.dataManager = dataManager;
        }
        #endregion

        #region Properties
        public CourseContainerViewModel Course { get; private set; } = null!;
        public ObservableCollection<ModuleItemViewModel> Modules { get; } = new();
        public override IViewModelDataParameter? DataParameter { get => Course; set => SetCourse(value); }
        #endregion

        #region Methods
        public override void Dispose()
        {
        }
        [RelayCommand] private void CreateModule()
        {
            try
            {
                var defaultModule = new ModuleItemViewModel(new ModuleItem(), Course);
                var pageNavigationRequest = new PageNavigationRequest(typeof(ModuleEditorViewModel), NavigationRequestAction.GoNext, defaultModule);
                eventAggregator.Publish(pageNavigationRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand] private void ImportModule()
        {

        }
        [RelayCommand] private void EditCourseInfo()
        {
            try
            {
                var pageNavigationRequest = new PageNavigationRequest(typeof(CourseEditorViewModel), NavigationRequestAction.GoNext, Course);
                eventAggregator.Publish(pageNavigationRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand] private void OpenModule(ModuleItemViewModel module)
        {

        }
        [RelayCommand] private void ExportModule(ModuleItemViewModel module)
        {

        }
        [RelayCommand] private void DeleteModule(ModuleItemViewModel module)
        {

        }
        #endregion

        #region Helpers
        private void SetCourse(IViewModelDataParameter? value)
        {
            try
            {
                if (value is not CourseContainerViewModel course) return;
                Course = course;
                OnPropertyChanged(nameof(Course));
                Header = course.Item.Name;
                eventAggregator.InvokeActionOnUIThread(() =>
                {
                    Modules.Clear();
                    var modules = dataManager.GetModulesData(course, 0, 10000);
                    foreach (var item in modules) { Modules.Add(item); }
                });
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
            
        }
        #endregion
    }
}
