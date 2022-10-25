using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsCryptoWidget.Helpers;

namespace WindowsCryptoWidget.ViewModels
{
    public class PairsViewModel : ObservableObject
    {
        #region Private Fields

        /// <inheritdoc cref="LastUpdateTime"/>
        private string _lastUpdateTime;

        /// <inheritdoc cref="PairsList"/>
        private ObservableCollection<PairModel> _pairsList = new ObservableCollection<PairModel>();

        /// <inheritdoc cref="UpdateDelay"/>
        private double _updateDelay;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Последнее обновление.
        /// </summary>
        public string LastUpdateTime
        {
            get => _lastUpdateTime;
            set => SetProperty(ref _lastUpdateTime, value);
        }

        /// <inheritdoc cref="Lastupdate"/>
        private DateTime _lastupdate;

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

        public CancellationTokenSource LoopCancellationTokenSource { get; }

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
        /// Задержка обновления.
        /// </summary>
        public double UpdateDelay
        {
            get => _updateDelay;
            set => SetProperty(ref _updateDelay, value);
        }

        #endregion Public Properties

        #region Public Constructors

        public PairsViewModel()
        {
            LoopCancellationTokenSource = new CancellationTokenSource();
            ConfigurePairsList();
            PairsList.CollectionChanged += (sender, args) => MoveToSettings();
            Task.Run(async () => UpdateLoop());
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

        private void ConfigurePairsList()
        {
            foreach (string pair in SettingsHelpers.SettingsConfig.FavPairs)
            {
                _pairsList.Add(new PairModel(pair));
            }
            AppEventsHelper.PairsCountChanged?.Invoke(PairsList.Count);
        }

        private void MoveToSettings()
        {
            SettingsHelpers.SettingsConfig.FavPairs.Clear();
            foreach (var pair in PairsList)
            {
                SettingsHelpers.SettingsConfig.FavPairs.Add(pair.Title);
            }
            JsonHelper.SaveJson(SettingsHelpers.SettingsConfig, SettingsHelpers.FavCursPath);
            AppEventsHelper.PairsCountChanged.Invoke(PairsList.Count);
        }

        #endregion Private Methods
    }
}