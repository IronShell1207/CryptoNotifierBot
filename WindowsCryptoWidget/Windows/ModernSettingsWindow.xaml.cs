using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ModernWpf.Controls;
using WindowsCryptoWidget.ViewModels;
using WindowsCryptoWidget.Windows.SettingsAreas;

namespace WindowsCryptoWidget.Windows
{
    /// <summary>
    /// Interaction logic for ModernSettingsWindow.xaml
    /// </summary>
    public partial class ModernSettingsWindow : Window, INotifyPropertyChanged
    {
        private PairsViewModel viewModel { get; }

        public List<NavigationViewItem> NavigationItems { get; set; } 

        public NavigationViewItem SelectedNavMenu { get; set; }
        public ModernSettingsWindow(PairsViewModel vm)
        {
            viewModel = vm;
            DataContext = viewModel;
            InitializeComponent();
            NavigationItems = new List<NavigationViewItem>()
            {
                new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Library), Name = "widgetSettings", Tag = "widgetSettings", Content = "Widget settings"},
                new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Library), Name = "pairsAndExchanges", Tag = "pairsAndExchanges", Content = "Pairs and exchanges"},
                new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Library), Name = "log", Tag = "log", Content = "Log"}
            };
            SelectedNavMenu = NavigationItems[0];
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SettingsNavigator_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (SelectedNavMenu == NavigationItems[0])
            {
                WidgetStyleSettings widgetSettings = new WidgetStyleSettings();
                ContentFrame.Navigate(widgetSettings);
            }
            else if (SelectedNavMenu == NavigationItems[1])
            {
                PairsAreaSettigs pairsArea = new PairsAreaSettigs();
                ContentFrame.Navigate(pairsArea);
            }
            else if (SelectedNavMenu == NavigationItems[2])
            {
                LogArea logArea = new LogArea();
                ContentFrame.Navigate(logArea);
            }
        }
    }
}