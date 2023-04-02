using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsCryptoWidget.ViewModels;

namespace WindowsCryptoWidget.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private PairsViewModel viewModel { get; }

        public SettingsWindow(PairsViewModel vm)
        {
            viewModel = vm;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void TB_CurName_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void SettingsWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseButtonState.Pressed == e.LeftButton)
                DragMove();
        }

        private void CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}