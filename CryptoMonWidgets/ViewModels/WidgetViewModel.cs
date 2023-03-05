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

        public ObservableCollection<PairViewModel> Pairs { get; set; } = new ObservableCollection<PairViewModel>();

        public RelayCommand AddPairCommand { get; }

        /// <inheritdoc cref="Scale"/>
        private double _scale = 0.5;

        /// <summary>
        /// Масштаб
        /// </summary>
        public double Scale
        {
            get => _scale;
            set
            {
                SetProperty(ref _scale, value);
                ScaleChanged?.Invoke();
            }
        }

        public void OnAddPair()
        {
        }

        public event Action ScaleChanged;

        public WidgetViewModel()
        {
            AddPairCommand = new RelayCommand(OnAddPair);
            Pairs.Add(new PairViewModel()
            {
                PairBaseName = "Btc",
                PairQuoteName = "usdt",
                Price = 30000
            });
            Pairs.Add(new PairViewModel()
            {
                PairBaseName = "Btc",
                PairQuoteName = "usdt",
                Price = 30000
            });
            Pairs.Add(new PairViewModel()
            {
                PairBaseName = "Btc",
                PairQuoteName = "usdt",
                Price = 30000
            });
            Pairs.Add(new PairViewModel()
            {
                PairBaseName = "Btc",
                PairQuoteName = "usdt",
                Price = 30000
            });
            Pairs.Add(new PairViewModel()
            {
                PairBaseName = "Btc",
                PairQuoteName = "usdt",
                Price = 30000
            });
        }

    }
}
