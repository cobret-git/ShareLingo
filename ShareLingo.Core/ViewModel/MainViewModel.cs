using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetForge.Core;
using ShareLingo.Core.Services;

namespace ShareLingo.Core.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields
        private readonly IEventAggregator eventAggregator;
        private readonly IBuildInfoManager buildManager;
        private readonly INavigationService navigationService;
        [ObservableProperty] private bool sidebarExpanded;
        #endregion

        #region Constructors
        public MainViewModel(IEventAggregator eventAggregator, IBuildInfoManager buildMng, INavigationService navigationService)
        {
            this.eventAggregator = eventAggregator;
            this.buildManager = buildMng;
            this.navigationService = navigationService;
            
            navigationService.NavigationChanged += NavigationServiceOnNavigationChanged;
        }
        #endregion

        #region Properties
        public bool IsDarkTheme { get => buildManager.Theme == ThemeVariant.Dark; }
        public bool IsHomePageNavigated { get; private set; }
        public bool IsLibraryPageNavigated { get; private set; }
        public bool IsSettingsPageNavigated { get; private set; }
        public bool IsStorePageNavigated { get; private set; }
        #endregion

        #region Methods
        [RelayCommand] private void SwitchAppicationTheme()
        {
            if (buildManager.Theme == ThemeVariant.Dark)
            {
                buildManager.ApplyTheme(ThemeVariant.Light);
            }
            else
            {
                buildManager.ApplyTheme(ThemeVariant.Dark);
            }
            OnPropertyChanged(nameof(IsDarkTheme));
        }
        [RelayCommand] private void NavigateTo(NavigationVariant navigation)
        {
            switch (navigation)
            {
                case NavigationVariant.Library:
                    navigationService.GoToRoot(typeof(CourseBrowserViewModel));
                    break;
                default: break;
            }
        }
        #endregion
        
        #region Handlers
        private void NavigationServiceOnNavigationChanged(object? sender, NavigationEventArgs e)
        {
            IsHomePageNavigated = false;
            IsLibraryPageNavigated = e.ViewModelType == typeof(CourseBrowserViewModel);
            IsSettingsPageNavigated = false;
            IsStorePageNavigated = false;
            
            OnPropertyChanged(nameof(IsHomePageNavigated));
            OnPropertyChanged(nameof(IsLibraryPageNavigated));
            OnPropertyChanged(nameof(IsSettingsPageNavigated));
            OnPropertyChanged(nameof(IsStorePageNavigated));
        }
        #endregion
    }
    public enum NavigationVariant
    {
        Home,
        Library,
        Settings,
        Store
    }
}
