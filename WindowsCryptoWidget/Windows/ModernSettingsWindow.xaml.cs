using System.Windows;
using WindowsCryptoWidget.ViewModels;

namespace WindowsCryptoWidget.Windows
{
    /// <summary>
    /// Interaction logic for ModernSettingsWindow.xaml
    /// </summary>
    public partial class ModernSettingsWindow : Window
    {
        private PairsViewModel viewModel { get; }

        public ModernSettingsWindow(PairsViewModel vm)
        {
            viewModel = vm;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}