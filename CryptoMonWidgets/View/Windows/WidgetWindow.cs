using Windows.Foundation;
using CryptoMonWidgets.View.Pages;
using CryptoMonWidgets.View.Windows.Base;
using CryptoMonWidgets.ViewModels;
using WinUIEx;

namespace CryptoMonWidgets.View.Windows
{
    public class WidgetWindow : BorderlessWindow
    {
        public WidgetViewModel ViewModel { get; set; }
        private Size _baseWindowSize = new Size(250, 76);
        public WidgetWindow(WidgetViewModel viewModel)
        {
            ViewModel = viewModel;
            CalculateWindowSize();
            ViewModel.ScaleChanged += CalculateWindowSize;
            WidgetWindowContentPage widgetContentPage = new WidgetWindowContentPage();
            widgetContentPage.DataContext = ViewModel;
            ContentFrame.Content = widgetContentPage;
        }

        private void CalculateWindowSize()
        {
            double height = (_baseWindowSize.Height + (ViewModel.Pairs.Count * 50)) * ViewModel.Scale;
            double width = _baseWindowSize.Width * ViewModel.Scale;
            this.SetWindowSize(width, height);
        }
    }
}