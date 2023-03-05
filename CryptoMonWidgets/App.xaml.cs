using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Threading;
using CryptoMonWidgets.ViewModels;
using WinUIEx;
using CryptoMonWidgets.Services;
using CryptoMonWidgets.Services.Interface;

namespace CryptoMonWidgets
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Mutex _appMutex;

        public static WindowEx MainWindow { get; private set; }

        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            
            CreateServices();
        }

        private void CreateServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<INavigationService, MainNavigationService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            if (BlockSecondApp())
            {
                MainWindow = new MainWindow();
                MainWindow.Activate();
            }
        }

        /// <summary>
        /// Блокирует второй запуск.
        /// </summary>
        private bool BlockSecondApp()
        {
            _appMutex = new Mutex(true, "CryWidgetBlockingStartMutex", out bool isNewInstance);
            if (!isNewInstance)
            {
                Environment.Exit(0);
                return false;
            }

            return true;
        }

        
    }
}