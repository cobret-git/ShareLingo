using Microsoft.Extensions.DependencyInjection;
using ShareLingo.Core.Services;
using ShareLingo.Core.ViewModel;
using ShareLingo.WPF.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using NetForge.Core;
using NetForge.Wpf;

namespace ShareLingo.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Constructors
    public App()
    {
        Services = ConfigureServices();
        this.InitializeComponent();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }
    #endregion

    #region Helpers
    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<IBuildInfoManager, BuildManager>();
        services.AddSingleton<IServiceLocator, ServiceLocator>();
        services.AddSingleton<INavigationService, WpfNavigationService>();
        services.AddSingleton<IDataManager, LiteDbManager>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<CourseBrowserViewModel>();
        services.AddTransient<CourseEditorViewModel>();

        return services.BuildServiceProvider();
    }
    #endregion

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Services.GetService<IBuildInfoManager>()?.ApplyTheme(ThemeVariant.Light);
        Services.GetService<IDataManager>()?.Open();
        // Initialize any required services or configurations here
    }
}

