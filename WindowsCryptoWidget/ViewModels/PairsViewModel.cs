using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using WindowsCryptoWidget.Helpers;

namespace WindowsCryptoWidget.ViewModels
{
    public class PairsViewModel : ObservableObject
    {

        #region Private Fields

        /// <inheritdoc cref="BackgroundTransparency"/>
        private double _backgroundTransparency = SettingsHelpers.SettingsConfig.WidgetOpacity;

        /// <inheritdoc cref="FontsTransparency"/>
        private double _fontsTransparency = SettingsHelpers.SettingsConfig.WidgetFontsOpacity;

        /// <inheritdoc cref="Lastupdate"/>
        private DateTime _lastupdate;

        /// <inheritdoc cref="LastUpdateTime"/>
        private string _lastUpdateTime;

        /// <inheritdoc cref="NewPairName"/>
        private string _newPairName;

        /// <inheritdoc cref="PairsList"/>
        private ObservableCollection<PairModel> _pairsList = new ObservableCollection<PairModel>();

        /// <inheritdoc cref="Transparency"/>
        private double _transparency = SettingsHelpers.SettingsConfig.WidgetOpacity;

        /// <inheritdoc cref="UpdateDelay"/>
        private double _updateDelay;

        /// <inheritdoc cref="WidgetScale"/>
        private double _widgetScale = SettingsHelpers.SettingsConfig.WidgetScale;

        #endregion Private Fields

        #region Public Properties

        /// <inheritdoc cref="AddPair"/>
        public RelayCommand AddPairCommand { get; }

        /// <summary>
        /// Прозрачность фона окна.
        /// </summary>
        public double BackgroundTransparency
        {
            get => _backgroundTransparency;
            set
            {
                SetProperty(ref _backgroundTransparency, value);
                SettingsHelpers.SettingsConfig.WidgetOpacity = value;
                JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            }
        }

        public ConsoleOutputsHelper ConsoleHelper { get; } = ConsoleOutputsHelper.Instance;

        /// <summary>
        /// Прозрачность шрифтов.
        /// </summary>
        public double FontsTransparency
        {
            get => _fontsTransparency;
            set
            {
                SetProperty(ref _fontsTransparency, value);
                SettingsHelpers.SettingsConfig.WidgetFontsOpacity = value;
                JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            }
        }

        /// <summary>
        /// Последнее время обновления.
        /// </summary>
        public DateTime Lastupdate
        {
            get => _lastupdate;
            set
            {
                SetProperty(ref _lastupdate, value);
                LastUpdateTime = value.ToLongTimeString();
            }
        }

        /// <summary>
        /// Последнее обновление.
        /// </summary>
        public string LastUpdateTime
        {
            get => _lastUpdateTime;
            set => SetProperty(ref _lastUpdateTime, value);
        }
        public CancellationTokenSource LoopCancellationTokenSource { get; }

        /// <summary>
        /// Текст для новой пары.
        /// </summary>
        public string NewPairName
        {
            get => _newPairName;
            set => SetProperty(ref _newPairName, value);
        }

        /// <summary>
        /// Список пар.
        /// </summary>
        public ObservableCollection<PairModel> PairsList
        {
            get => _pairsList;
            set
            {
                SetProperty(ref _pairsList, value);
                AppEventsHelper.PairsCountChanged.Invoke(value.Count);
            }
        }

        /// <summary>
        /// Прозрачность виджета
        /// </summary>
        public double Transparency
        {
            get => _transparency;
            set
            {
                SetProperty(ref _transparency, value);
                JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            }
        }

        /// <summary>
        /// Задержка обновления.
        /// </summary>
        public double UpdateDelay
        {
            get => _updateDelay;
            set => SetProperty(ref _updateDelay, value);
        }

        /// <summary>
        /// Скалирование виджета.
        /// </summary>
        public double WidgetScale
        {
            get => _widgetScale;
            set
            {
                SetProperty(ref _widgetScale, value);
                ScaleChanged?.Invoke();
                SettingsHelpers.SettingsConfig.WidgetScale = value;
                JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            }
        }

        #endregion Public Properties

        #region Public Events

        public event Action ScaleChanged;

        #endregion Public Events

        #region Public Constructors

        public PairsViewModel()
        {
            LoopCancellationTokenSource = new CancellationTokenSource();
            ConfigurePairsList();
            PairsList.CollectionChanged += (sender, args) => MoveToSettings();
            Task.Run(async () => UpdateLoop());
            AddPairCommand = new RelayCommand(AddPair, CanExecuteAddPairCommand);
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task UpdateLoop()
        {
            while (!LoopCancellationTokenSource.IsCancellationRequested)
            {
                if (PairsList.Count > 0 && ExchangesHelper.LocalDataRequester.DataAvailable)
                {
                    foreach (var pair in PairsList)
                    {
                        string pairBase = pair.Title.Split("/").First();
                        string pairQuote = pair.Title.Split("/").Last();
                        var pairFromExchange = await ExchangesHelper.LocalDataRequester.GetKucoinData(pairBase, pairQuote);
                        if (pairFromExchange != null && pair.Price != double.Parse(pairFromExchange.Price))
                        {
                            double lastPrice = pair.Price;
                            pair.Price = double.Parse(pairFromExchange.Price);
                            pair.IsPumping = lastPrice < pair.Price;
                            pair.ArrowSymbol = lastPrice < pair.Price ? "▲" : "▼";
                            pair.PriceChangingDouble = double.Parse(pairFromExchange.changePrice);
                            pair.ProcentDoubleChanging = Math.Round(double.Parse(pairFromExchange.changeRate) * 100, 3);
                            Lastupdate = DateTime.Now;
                        }
                    }
                }

                await Task.Delay(500);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Добавить пару.
        /// </summary>
        private void AddPair()
        {
            PairsList.Add(new PairModel(NewPairName));
        }

        /// <summary>
        /// Может ли исполнить команду <see cref="AddPairCommand"/>.
        /// </summary>
        private bool CanExecuteAddPairCommand()
        {
            return true;
        }

        private void ConfigurePairsList()
        {
            foreach (string pair in SettingsHelpers.SettingsConfig.SavedPairs)
            {
                _pairsList.Add(new PairModel(pair));
            }
            AppEventsHelper.PairsCountChanged?.Invoke(PairsList.Count);
        }

        private void MoveToSettings()
        {
            SettingsHelpers.SettingsConfig.SavedPairs.Clear();
            foreach (var pair in PairsList)
            {
                SettingsHelpers.SettingsConfig.SavedPairs.Add(pair.Title);
            }
            JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            AppEventsHelper.PairsCountChanged.Invoke(PairsList.Count);
        }

        #endregion Private Methods
    }
}