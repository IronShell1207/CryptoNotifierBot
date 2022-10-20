using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsCryptoWidget.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region Название пары

        private string _TitlePair = "BTCUSDT";

        public string TitlePair
        {
            get => _TitlePair;
            set => SetProperty(ref _TitlePair, value);
        }

        #endregion Название пары

        #region Цена пары

        private string _PairCurr = "0.00";

        public string PairCurr
        {
            get => _PairCurr;
            set => SetProperty(ref _PairCurr, value);
        }

        #endregion Цена пары
    }
}