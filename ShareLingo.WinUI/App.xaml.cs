using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using NetForge.Core;
using NetForge.WinUI;
using ShareLingo.Core.Services;
using ShareLingo.Core.ViewModel;
using ShareLingo.WinUI.Services;
using ShareLingo.WinUI.View;
using System;
using System.IO.Abstractions;

namespace ShareLingo.WinUI
{
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            Services.GetService<IDataManager>()?.Open();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IBuildInfoManager, BuildManager>();
            services.AddSingleton<IDataManager, LiteDbManager>();
            services.AddSingleton<IServiceLocator, ShareLingoServiceLocator>();
            services.AddSingleton<IMessageManager, ContentDialogHost>();

            services.AddTransient<CourseBrowserViewModel>();
            services.AddTransient<CourseEditorViewModel>();
            services.AddTransient<CourseViewerViewModel>();

            return services.BuildServiceProvider();
        }

        private Window? m_window;
    }
}
