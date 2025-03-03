using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using NetForge.Core;
using ShareLingo.WinUI.Services;
using ShareLingo.WinUI.View;
using System;

namespace ShareLingo.WinUI
{
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
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

            return services.BuildServiceProvider();
        }



        private Window m_window;
    }
}
