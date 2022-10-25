using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CryptoApi.Constants;
using WindowsCryptoWidget.Controls;
using WindowsCryptoWidget.Helpers;
using WindowsCryptoWidget.ViewModels;
using WindowsCryptoWidget.Windows;

namespace WindowsCryptoWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppEventsHelper.PairsCountChanged += Model_PairsCountChanged;
            var model = new PairsViewModel();
            DataContext = model;
            InitializeComponent();
            Model_PairsCountChanged(model.PairsList.Count);
        }

        private void Model_PairsCountChanged(int obj)
        {
            this.Height = 45 + (obj * 94);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseButtonState.Pressed == e.LeftButton)
                DragMove();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }

        private double lastpos = 0;

        private void ButtonRestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Left = lastpos;
            Ushko.Visibility = Visibility.Collapsed;
            this.Width = 140;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lastpos = this.Left;
            this.Left = -190;
            Ushko.Visibility = Visibility.Visible;
            this.Width = 240;
        }
    }
}