﻿using System.Windows;
using System.Windows.Controls;
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
        private object HiddenTemplate { get; set; }
        private object MainContent { get; set; }
        private PairsViewModel MainViewModel { get; }

        private (double, double) MainWindowLastPoint { get; set; }

        private (double, double) MainWindowSize { get; set; }

        #endregion Private Properties

        #region Public Constructors

        public MainWindow()
        {
            MainViewModel = new PairsViewModel();
            DataContext = MainViewModel;
            InitializeComponent();

            WidgetWindowHelper.Instance.Initialize(this);

            AppEventsHelper.PairsCountChanged += Model_PairsCountChanged;
            Model_PairsCountChanged(MainViewModel.PairsList.Count);
            MainViewModel.ScaleChanged += Model_ScaleChanged;
            MainViewModel.StyleChanged += MainViewModel_StyleChanged;
            Loaded += (s, e) => MainViewModel_StyleChanged();
            Model_ScaleChanged();
            MainGrid.Width = MainViewModel.UsedStyle.DataWidth;
        }

        private void MainViewModel_StyleChanged()
        {
            MainGrid.Width = MainViewModel.UsedStyle.DataWidth;
            Model_PairsCountChanged(MainViewModel.PairsList.Count);
            Model_ScaleChanged();
        }

        #endregion Public Constructors

        #region Private Methods

        private void ButtonCollapceOnClick(object sender, RoutedEventArgs e)
        {
            MainContent = this.Content;
            MainWindowSize = (Width, Height);
            MainWindowLastPoint = (Left, Top);
            Content = null;
            var restoreButton = new Button()
            {
                Width = 30,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Content = ">",
            };
            restoreButton.Click += ButtonRestoreWindow_Click;
            Content = restoreButton;
            HiddenTemplate = Content;
            Left = 0;
            Width = 30;
            Height = 30;
        }

        private void ButtonRestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            Content = MainContent;
            Width = MainWindowSize.Item1;
            Height = MainWindowSize.Item2;
            Left = MainWindowLastPoint.Item1;
            Top = MainWindowLastPoint.Item2;
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            //SettingsWindow sw = new SettingsWindow(DataContext as PairsViewModel);
            ModernSettingsWindow sw = new ModernSettingsWindow(DataContext as PairsViewModel);
            // SettingsWindow sw = new SettingsWindow(DataContext as PairsViewModel);
            sw.ShowDialog();
        }

        private void Model_PairsCountChanged(int obj)
        {
            if (MainGrid != null)
            {
                MainGrid.Height = 45 + (obj * MainViewModel.UsedStyle.DataHeight);
                BaseHeight = MainGrid.Height;
            }
        }

        private void Model_ScaleChanged()
        {
            this.Width = MainViewModel.UsedStyle.DataWidth * MainViewModel.WidgetScale;
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