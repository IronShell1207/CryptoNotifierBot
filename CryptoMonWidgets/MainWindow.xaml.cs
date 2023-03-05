using CryptoMonWidgets.View.Pages;
using CryptoMonWidgets.View.Windows.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace CryptoMonWidgets
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Activated += OnMainWindow_Activated;
        }

        private void OnMainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            Content = null;
            Content = new Frame();
            (Content as Frame).Navigate(typeof(RootView));
            Activated -= OnMainWindow_Activated;
        }
    }
}