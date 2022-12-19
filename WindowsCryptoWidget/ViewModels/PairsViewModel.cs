using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Effects;
using CryptoApi.Constants;
using CryptoApi.Objects.ExchangesPairs;
using WindowsCryptoWidget.Helpers;
using WindowsCryptoWidget.Models;

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

        /// <inheritdoc cref="SelectedStyle"/>
        private int _selectedStyle = SettingsHelpers.SettingsConfig.SelectedStyleIndex;

        /// <inheritdoc cref="Transparency"/>
        private double _transparency = SettingsHelpers.SettingsConfig.WidgetOpacity;

        /// <inheritdoc cref="UpdateDelay"/>
        private double _updateDelay = SettingsHelpers.SettingsConfig.DataUpdateInterval.TotalSeconds;

        /// <inheritdoc cref="WidgetScale"/>
        private double _widgetScale = SettingsHelpers.SettingsConfig.WidgetScale;

        public List<ExchangeEnum> ExchangeList { get; set; }

        private ExchangeEnum _selectedExchange = SettingsHelpers.SettingsConfig.UsedExchange;

        public ExchangeEnum SelectedExchange
        {
            get
            {
                return _selectedExchange;
            }
            set
            {
                SetProperty(ref _selectedExchange, value);
                if (value.GetType() == typeof(ExchangeEnum))
                {
                    SettingsHelpers.SettingsConfig.UsedExchange = _selectedExchange;
                    JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
                }
            }
        }

        #endregion Private Fields

        #region Public Properties

        /// <inheritdoc cref="AddPair"/>
        public RelayCommand AddPairCommand { get; }

        public WidgetStyle UsedStyle { get; set; } = WidgetStyles.DefaultWidgetStyle;

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
        /// Выбранный стиль.
        /// </summary>
        public int SelectedStyle
        {
            get => _selectedStyle;
            set
            {
                SetProperty(ref _selectedStyle, value);
                if (value > -1)
                {
                    UsedStyle = WidgetStyles.AllStyles.First(x => x.Index == value);
                    OnPropertyChanged(nameof(UsedStyle));
                    SettingsHelpers.SettingsConfig.SelectedStyleIndex = value;
                    JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
                    StyleChanged?.Invoke();
                }
            }
        }

        public event Action StyleChanged;

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
            set
            {
                SetProperty(ref _updateDelay, value);
                SettingsHelpers.SettingsConfig.DataUpdateInterval = TimeSpan.FromSeconds(_updateDelay);
                JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            }
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
            ExchangeList = new List<ExchangeEnum>()
            {
                ExchangeEnum.Okx, ExchangeEnum.Kucoin, ExchangeEnum.Binance, ExchangeEnum.Bitget, ExchangeEnum.GateIO
            };
            ExchangesHelper.Instance.StartLoop();
            LoopCancellationTokenSource = new CancellationTokenSource();
            ConfigurePairsList();
            PairsList.CollectionChanged += (sender, args) => MoveToSettings();
            Task.Run(UpdateLoop);
            AddPairCommand = new RelayCommand(AddPair, CanExecuteAddPairCommand);
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task UpdateLoop()
        {
            while (!LoopCancellationTokenSource.IsCancellationRequested)
            {
                if (PairsList.Count > 0 && ExchangesHelper.Instance.LatestDataSet != null)
                {
                    try
                    {
                        switch (SettingsHelpers.SettingsConfig.UsedExchange)
                        {
                            case ExchangeEnum.Okx:
                                HandleOkxData(ExchangesHelper.Instance.GetOkxLatestData());
                                break;

                            case ExchangeEnum.Binance:
                                if (SelectedStyle == 0)
                                {
                                    SelectedStyle = 1;
                                }

                                HandleBinanceData(ExchangesHelper.Instance.GetBinanceLatestData());
                                break;

                            case ExchangeEnum.Bitget:
                                HandleBitgetData(ExchangesHelper.Instance.GetBitgetLatestData());
                                break;

                            case ExchangeEnum.Kucoin:
                                HandleKucointData(ExchangesHelper.Instance.GetKucoinLatestData());
                                break;

                            default:
                                HandleOkxData(ExchangesHelper.Instance.GetOkxLatestData());
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                await Task.Delay(500);
            }
        }

        private void HandleOkxData(List<OkxTicker> data)
        {
            foreach (var pair in PairsList)
            {
                string pairBase = pair.Title.Split("/").First();
                string pairQuote = pair.Title.Split("/").Last();
                var pairFromExchange = data.FirstOrDefault(x => x.Symbol == ExchangesSpotLinks.GetPairConverted(Exchanges.Okx, pairBase, pairQuote));
                if (pairFromExchange != null && pair.Price != double.Parse(pairFromExchange.Price))
                {
                    double lastPrice = pair.Price;
                    pair.Price = double.Parse(pairFromExchange.Price);
                    pair.IsPumping = lastPrice < pair.Price;
                    pair.ArrowSymbol = lastPrice < pair.Price ? "▲" : "▼";
                    pair.Open24h = double.Parse(pairFromExchange.open24h);
                    //pair.PriceChangingDouble = double.Parse(pairFromExchange.changePrice);
                    //pair.ProcentDoubleChanging = Math.Round(double.Parse(pairFromExchange.changeRate) * 100, 3);
                    Lastupdate = DateTime.Now;
                }
            }
        }

        private void HandleBinanceData(List<BinancePair> data)
        {
            foreach (var pair in PairsList)
            {
                string pairBase = pair.Title.Split("/").First();
                string pairQuote = pair.Title.Split("/").Last();
                var pairFromExchange = data.FirstOrDefault(x => x.Symbol == ExchangesSpotLinks.GetPairConverted(Exchanges.Binance, pairBase, pairQuote));
                if (pairFromExchange != null && pair.Price != double.Parse(pairFromExchange.Price))
                {
                    double lastPrice = pair.Price;
                    pair.Price = double.Parse(pairFromExchange.Price);
                    pair.IsPumping = lastPrice < pair.Price;
                    pair.ArrowSymbol = lastPrice < pair.Price ? "▲" : "▼";
                    //pair.Open24h = double.Parse(pairFromExchange.open24h);
                    //pair.PriceChangingDouble = double.Parse(pairFromExchange.changePrice);
                    //pair.ProcentDoubleChanging = Math.Round(double.Parse(pairFromExchange.changeRate) * 100, 3);
                    Lastupdate = DateTime.Now;
                }
            }
        }

        private void HandleBitgetData(List<BitgetTicker> data)
        {
            foreach (var pair in PairsList)
            {
                string pairBase = pair.Title.Split("/").First();
                string pairQuote = pair.Title.Split("/").Last();
                var pairFromExchange = data.FirstOrDefault(x => x.Symbol == ExchangesSpotLinks.GetPairConverted(Exchanges.Bitget, pairBase, pairQuote));
                if (pairFromExchange != null && pair.Price != double.Parse(pairFromExchange.Price))
                {
                    double lastPrice = pair.Price;
                    pair.Price = double.Parse(pairFromExchange.Price);
                    pair.IsPumping = lastPrice < pair.Price;
                    pair.ArrowSymbol = lastPrice < pair.Price ? "▲" : "▼";
                    //pair.Open24h = double.Parse(pairFromExchange.open24h);
                    //pair.PriceChangingDouble = double.Parse(pairFromExchange.changePrice);
                    //pair.ProcentDoubleChanging = Math.Round(double.Parse(pairFromExchange.changeRate) * 100, 3);
                    Lastupdate = DateTime.Now;
                }
            }
        }

        private void HandleKucointData(List<KuTicker> data)
        {
            foreach (var pair in PairsList)
            {
                string pairBase = pair.Title.Split("/").First();
                string pairQuote = pair.Title.Split("/").Last();
                var pairFromExchange = data.FirstOrDefault(x => x.Symbol == ExchangesSpotLinks.GetPairConverted(Exchanges.Kucoin, pairBase, pairQuote));
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
                var pairModel = new PairModel(pair);
                pairModel.RemoveRequested += OnPairModel_RemoveRequested;
                _pairsList.Add(pairModel);
            }
            AppEventsHelper.PairsCountChanged?.Invoke(PairsList.Count);
        }

        private void OnPairModel_RemoveRequested(PairModel obj)
        {
            PairsList.Remove(obj);
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