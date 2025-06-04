using CONTENT = ShareLingo.Core.Resources.Content;
using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using ShareLingo.Core.Model;
using ShareLingo.Core.Services;
using System.Collections.ObjectModel;

namespace ShareLingo.Core.ViewModel
{
    public partial class CourseBrowserViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IDataManager dataManager;
        private bool loading = false;
        #endregion

        #region Constructors
        public CourseBrowserViewModel(IEventAggregator eventAggregator, IDataManager dataManager)
            : base(eventAggregator)
        {
            this.dataManager = dataManager;
            Header = CONTENT.courseBrowser_header;
            Courses = new(dataManager.GetCourseData(0, 200));
        }
        #endregion

        #region Properties
        public bool Loaded { get; private set; }
        public bool Loading { get => loading; set { loading = value; OnPropertyChanged(); } }
        public ObservableCollection<CourseContainerViewModel> Courses { get; } = new();
        #endregion

        #region Methods
        [RelayCommand] private void CreateCourse()
        {
            try
            {
                var defaultCourse = new CourseContainerViewModel(new CourseContainer());
                var pageNavigationRequest = new PageNavigationRequest(typeof(CourseEditorViewModel), NavigationAction.Forward, defaultCourse);
                eventAggregator.Publish(pageNavigationRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        [RelayCommand] private void OpenCourse(CourseContainerViewModel course)
        {
            try
            {
                if (course == null) throw new ArgumentNullException(nameof(course));
                var pageNavigationRequest = new PageNavigationRequest(typeof(CourseViewerViewModel), NavigationAction.Forward, course);
                eventAggregator.Publish(pageNavigationRequest);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
        }
        public override void Dispose()
        {
        }
        #endregion

        #region Handlers
        [RelayCommand] private async Task OnLoaded()
        {
            try
            {
                if (Loaded || Loading) return;
                Loading = true;
                CourseContainerViewModel[] data = Array.Empty<CourseContainerViewModel>();
                await Task.Run(() => data = dataManager.GetCourseData(0, 1_000));
                foreach (var item in data) Courses.Add(item);
            }
            catch (Exception ex) { eventAggregator.Publish(LoggedData.Debug(ex)); }
            Loaded = true;
            Loading = false;
        }
        #endregion
    }
}
