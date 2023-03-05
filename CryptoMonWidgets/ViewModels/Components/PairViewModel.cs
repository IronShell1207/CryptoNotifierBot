using CommunityToolkit.Mvvm.ComponentModel;

namespace CryptoMonWidgets.ViewModels.Components
{
    public class PairViewModel : ObservableObject
    {
        private bool _lastChangeSide;
        private string _pairBaseName;

        private string _pairQuoteName;

        private double _price;

        private double _priceChange;

        private double _priceChangePercent;

        public bool LastChangeSide
        {
            get => _lastChangeSide;
            set => SetProperty(ref _lastChangeSide, value);
        }

        public string PairBaseName
        {
            get => _pairBaseName;
            set => SetProperty(ref _pairBaseName, value);
        }

        public string PairQuoteName
        {
            get => _pairQuoteName;
            set => SetProperty(ref _pairQuoteName, value);
        }

        public double Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public double PriceChange
        {
            get => _priceChange;
            set => SetProperty(ref _priceChange, value);
        }

        public double PriceChangePercent
        {
            get => _priceChangePercent;
            set => SetProperty(ref _priceChangePercent, value);
        }
    }
}