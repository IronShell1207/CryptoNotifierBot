using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CryptoMonWidgets.Services;

namespace CryptoMonWidgets.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<WidgetViewModel> Widgets { get; set; } =
            new ObservableCollection<WidgetViewModel>();

        /// <inheritdoc cref="SelectedWidgetSettings"/>
        private WidgetViewModel _selectedWidgetSettings;

        /// <summary>
        /// Выбранный
        /// </summary>
        public WidgetViewModel SelectedWidgetSettings
        {
            get => _selectedWidgetSettings;
            set => SetProperty(ref _selectedWidgetSettings, value);
        }   
        public RelayCommand AddWidgetCommand { get; }

        public void OnAddWidget()
        {
            var newWidgetModel = new WidgetViewModel();
            Widgets.Add(newWidgetModel);
            _widgetWindowsService.CreateWidget(newWidgetModel);

        }
        private WidgetWindowsService _widgetWindowsService;
        public MainViewModel(WidgetWindowsService widgetService)
        {
            _widgetWindowsService = widgetService;
            AddWidgetCommand = new RelayCommand(OnAddWidget);
        }
    }
}