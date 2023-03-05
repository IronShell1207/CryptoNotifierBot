using CryptoMonWidgets.View.Pages;
using CryptoMonWidgets.View.Windows.Base;
using CryptoMonWidgets.ViewModels;
using System.Collections.Generic;
using CryptoMonWidgets.View.Windows;

namespace CryptoMonWidgets.Services
{
    public class WidgetWindowsService
    {
        public List<object> CreatedWidgets { get; private set; } = new List<object>();

        public void CreateWidget(WidgetViewModel viewModel)
        {
            WidgetWindow widgetWindow = new WidgetWindow(viewModel);
            widgetWindow.Activate();
        }
    }
}