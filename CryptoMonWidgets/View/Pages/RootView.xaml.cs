using CryptoMonWidgets.Models;
using CryptoMonWidgets.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CryptoMonWidgets.View.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RootView : Page
    {
        public RootView()
        {
            this.InitializeComponent();
            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(AppTitleBar);
            App.MainWindow.Activated += MainWindow_Activated;
            Loaded += OnRootView_Loaded;
        }

        private void OnRootView_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBarHelper.UpdateTitleBar(RequestedTheme);
            App.ServiceProvider.GetRequiredService<INavigationService>().ViewFrame = ContentFrame;
            App.ServiceProvider.GetRequiredService<INavigationService>().NavigateTo(typeof(MainPage), null);
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

            AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
        }

        private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            AppTitleBar.Margin = new Thickness()
            {
                Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
                Top = AppTitleBar.Margin.Top,
                Right = AppTitleBar.Margin.Right,
                Bottom = AppTitleBar.Margin.Bottom
            };
        }
    }
}