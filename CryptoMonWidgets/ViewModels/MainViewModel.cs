using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CryptoMonWidgets.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<WidgetViewModel> Widgets { get; set; } =
            new ObservableCollection<WidgetViewModel>();

        public RelayCommand AddWidgetCommand { get; }

        public void OnAddWidget()
        {
            Widgets.Add(new WidgetViewModel());
        }

        public MainViewModel()
        {
            AddWidgetCommand = new RelayCommand(OnAddWidget);
        }
    }
}