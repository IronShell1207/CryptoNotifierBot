using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsCryptoWidget.ViewModels
{
    public class PairModel : ObservableObject
    {
        #region Private Fields

        /// <inheritdoc cref="ArrowSymbol"/>
        private string _arrowSymbol;

        /// <inheritdoc cref="IsPumping"/>
        private bool _isPumping;

        private double _price = 0.00;

        /// <inheritdoc cref="PriceChangingDouble"/>
        private double _priceChangingDouble;

        /// <inheritdoc cref="ProcentChanging"/>
        private string _procentChanging;

        /// <inheritdoc cref="ProcentDoubleChanging"/>
        private double _procentDoubleChanging;

        private string _title = "BTCUSDT";

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Стрелка направления.
        /// </summary>
        public string ArrowSymbol
        {
            get => _arrowSymbol;
            set => SetProperty(ref _arrowSymbol, value);
        }

        /// <summary>
        /// Рост или падение.
        /// </summary>
        public bool IsPumping
        {
            get => _isPumping;
            set => SetProperty(ref _isPumping, value);
        }

        public double Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        /// <summary>
        /// Изменение цены.
        /// </summary>
        public double PriceChangingDouble
        {
            get => _priceChangingDouble;
            set { SetProperty(ref _priceChangingDouble, value); }
        }

        /// <summary>
        /// Процент изменения.
        /// </summary>
        public string ProcentChanging
        {
            get => _procentChanging;
            set => SetProperty(ref _procentChanging, value);
        }

        /// <summary>
        /// Процент изменения.
        /// </summary>
        public double ProcentDoubleChanging
        {
            get => _procentDoubleChanging;
            set
            {
                SetProperty(ref _procentDoubleChanging, value);
                ProcentChanging = $"{value}%";
            }
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        #endregion Public Properties

        #region Public Constructors

        public PairModel(string title)
        {
            Title = title;
        }

        public PairModel()
        {
        }

        #endregion Public Constructors
    }
}