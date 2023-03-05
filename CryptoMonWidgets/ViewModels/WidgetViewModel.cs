using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoMonWidgets.Enums;
using CryptoMonWidgets.ViewModels.Components;

namespace CryptoMonWidgets.ViewModels
{
    public class WidgetViewModel : ObservableObject
    {
        private WidgetStyleEnum _styleEnum;

        public WidgetStyleEnum StyleEnum
        {
            get => _styleEnum;
            set => SetProperty(ref _styleEnum, value);
        }
        
        public ObservableCollection<PairViewModel> Pairs { get; set; }

        public RelayCommand AddPairCommand { get; }

        public void OnAddPair()
        {
        }

        public WidgetViewModel()
        {
            AddPairCommand = new RelayCommand(OnAddPair);
        }

    }
}
