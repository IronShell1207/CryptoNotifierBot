using System.Windows;
using System.Windows.Input;
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
        #region Private Fields

        private double lastpos = 0;

        #endregion Private Fields

        #region Private Properties

        private double BaseHeight { get; set; }
        private PairsViewModel MainViewModel { get; }

        #endregion Private Properties

        #region Public Constructors

        public MainWindow()
        {
            MainViewModel = new PairsViewModel();
            DataContext = MainViewModel;
            InitializeComponent();
            AppEventsHelper.PairsCountChanged += Model_PairsCountChanged;
            Model_PairsCountChanged(MainViewModel.PairsList.Count);
            MainViewModel.ScaleChanged += Model_ScaleChanged;
            Model_ScaleChanged();
        }

        #endregion Public Constructors

        #region Private Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lastpos = this.Left;
            this.Left = -190;
            Ushko.Visibility = Visibility.Visible;
            this.Width = 240;
        }

        private void ButtonRestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Left = lastpos;
            Ushko.Visibility = Visibility.Collapsed;
            this.Width = 140;
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(DataContext as PairsViewModel);
            sw.ShowDialog();
        }

        private void Model_PairsCountChanged(int obj)
        {
            if (MainGrid != null)
            {
                MainGrid.Height = 45 + (obj * 94);
                BaseHeight = MainGrid.Height;
            }
        }

        private void Model_ScaleChanged()
        {
            this.Width = 140 * MainViewModel.WidgetScale;
            this.Height = BaseHeight * MainViewModel.WidgetScale;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseButtonState.Pressed == e.LeftButton)
                DragMove();
        }

        #endregion Private Methods
    }
}