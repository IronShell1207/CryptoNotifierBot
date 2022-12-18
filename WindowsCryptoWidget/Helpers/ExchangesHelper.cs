using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using CryptoApi.API;
using CryptoApi.Constants;
using CryptoApi.Objects.ExchangesPairs;
using CryptoApi.Static.DataHandler;
using WindowsCryptoWidget.Models;

namespace WindowsCryptoWidget.Helpers
{
    public class ExchangesHelper
    {
        /// <summary>
        /// Статичный экземпляр класса <see cref="ExchangesHelper"/>.
        /// </summary>
        private static readonly Lazy<ExchangesHelper> _instance = new((() => new ExchangesHelper()));

        /// <summary>
        /// Статичный экземпляр класса <see cref="ExchangesHelper"/>.
        /// </summary>
        public static ExchangesHelper Instance => _instance.Value;

        private bool _isAlive = false;

        private CancellationTokenSource _cancellationTokenSource;

        public DateTime LastDataUpdateTime { get; set; } = default;

        private object _latestData;

        public object LatestDataSet
        {
            get
            {
                return _latestData;
            }
            set
            {
                if (value != _latestData)
                {
                    LastDataUpdateTime = DateTime.Now;
                    _latestData = value;
                }
            }
        }

        public void StartLoop()
        {
            if (!_isAlive)
            {
                _isAlive = true;
                _cancellationTokenSource = new CancellationTokenSource();
                Task.Run(DataUpdateLoop);
            }
            else
            {
                throw new InvalidCastException("Loop was started!");
            }
        }

        public List<KuTicker> GetKucoinLatestData()
        {
            if (SettingsHelpers.SettingsConfig.UsedExchange == ExchangeEnum.Kucoin && _isAlive)
            {
                if (LatestDataSet.GetType() == typeof(KucoinData))
                {
                    KucoinData data = (KucoinData)LatestDataSet;
                    if (data != null)
                    {
                        return data.data.ticker.ToList();
                    }
                }
            }

            if (SettingsHelpers.SettingsConfig.UsedExchange != ExchangeEnum.Kucoin)
                throw new InvalidCastException(
                    $"Current exchange is not Kucoin, its: {SettingsHelpers.SettingsConfig.UsedExchange.ToString()}");
            if (!_isAlive)
                throw new InvalidCastException("Loop was not started!");
            return new List<KuTicker>();
        }

        public List<OkxTicker> GetOkxLatestData()
        {
            if (SettingsHelpers.SettingsConfig.UsedExchange == ExchangeEnum.Okx && _isAlive)
            {
                if (LatestDataSet.GetType() == typeof(OkxData))
                {
                    OkxData data = (OkxData)LatestDataSet;
                    if (data != null)
                    {
                        return data.data.ToList();
                    }
                }
            }

            if (SettingsHelpers.SettingsConfig.UsedExchange != ExchangeEnum.Okx)
                throw new InvalidCastException(
                    $"Current exchange is not Okx, its: {SettingsHelpers.SettingsConfig.UsedExchange.ToString()}");
            if (!_isAlive)
                throw new InvalidCastException("Loop was not started!");
            return new List<OkxTicker>();
        }

        public List<BinancePair> GetBinanceLatestData()
        {
            if (SettingsHelpers.SettingsConfig.UsedExchange == ExchangeEnum.Binance && _isAlive)
            {
                if (LatestDataSet.GetType() == typeof(List<BinancePair>))
                {
                    List<BinancePair> data = (List<BinancePair>)LatestDataSet;
                    if (data != null)
                    {
                        return data;
                    }
                }
            }

            if (SettingsHelpers.SettingsConfig.UsedExchange != ExchangeEnum.Binance)
                throw new InvalidCastException(
                    $"Current exchange is not Binance, its: {SettingsHelpers.SettingsConfig.UsedExchange.ToString()}");
            if (!_isAlive)
                throw new InvalidCastException("Loop was not started!");
            return new List<BinancePair>();
        }

        public List<BitgetTicker> GetBitgetLatestData()
        {
            if (SettingsHelpers.SettingsConfig.UsedExchange == ExchangeEnum.Bitget && _isAlive)
            {
                if (LatestDataSet.GetType() == typeof(BitgetData))
                {
                    BitgetData data = (BitgetData)LatestDataSet;
                    if (data != null)
                    {
                        return data.data.ToList();
                    }
                }
            }

            if (SettingsHelpers.SettingsConfig.UsedExchange != ExchangeEnum.Bitget)
                throw new InvalidCastException(
                    $"Current exchange is not Bitget, its: {SettingsHelpers.SettingsConfig.UsedExchange.ToString()}");
            if (!_isAlive)
                throw new InvalidCastException("Loop was not started!");
            return new List<BitgetTicker>();
        }

        public List<GateIOTicker> GetGateIOLatestData()
        {
            if (SettingsHelpers.SettingsConfig.UsedExchange == ExchangeEnum.GateIO && _isAlive)
            {
                if (LatestDataSet.GetType() == typeof(List<GateIOTicker>))
                {
                    List<GateIOTicker> data = (List<GateIOTicker>)LatestDataSet;
                    if (data != null)
                    {
                        return data;
                    }
                }
            }

            if (SettingsHelpers.SettingsConfig.UsedExchange != ExchangeEnum.GateIO)
                throw new InvalidCastException(
                    $"Current exchange is not Bitget, its: {SettingsHelpers.SettingsConfig.UsedExchange.ToString()}");
            if (!_isAlive)
                throw new InvalidCastException("Loop was not started!");
            return new List<GateIOTicker>();
        }

        public void StopLoop()
        {
            if (_isAlive)
            {
                _isAlive = false;
                _cancellationTokenSource.Cancel();
            }
            else
            {
                throw new InvalidCastException("Loop was not started!");
            }
        }

        /// <summary>
        /// Цикл обновления данных
        /// </summary>
        /// <returns></returns>
        private async Task DataUpdateLoop()
        {
            while (_isAlive && !_cancellationTokenSource.IsCancellationRequested)
            {
                //while (_updateLocker.CurrentCount == 0)
                //{
                //    await Task.Delay(20);
                //}

                object prevData = LatestDataSet;
                try
                {
                    object newList = SettingsHelpers.SettingsConfig.UsedExchange switch
                    {
                        (ExchangeEnum.Okx) => await new ExchangeApi(Exchanges.Okx).GetDataFromExchange<OkxData>(
                            _cancellationTokenSource.Token),
                        (ExchangeEnum.Binance) => await new ExchangeApi(Exchanges.Binance)
                            .GetDataFromExchange<List<BinancePair>>(_cancellationTokenSource.Token),
                        (ExchangeEnum.Kucoin) =>
                            await new ExchangeApi(Exchanges.Kucoin).GetDataFromExchange<KucoinData>(_cancellationTokenSource.Token),
                        (ExchangeEnum.Bitget) => await new ExchangeApi(Exchanges.Bitget).GetDataFromExchange<BitgetData>(
                                _cancellationTokenSource.Token),
                        (ExchangeEnum.GateIO) => await new ExchangeApi(Exchanges.GateIO)
                            .GetDataFromExchange<List<GateIOTicker>>(_cancellationTokenSource.Token),
                        _ => await new ExchangeApi(Exchanges.Okx).GetDataFromExchange<OkxData>(_cancellationTokenSource
                            .Token)
                    };
                    if (newList != null)
                    {
                        LatestDataSet = new object();
                        LatestDataSet = newList;
                    }
                }
                catch (Exception ex)
                {
                }

                await Task.Delay(SettingsHelpers.SettingsConfig.DataUpdateInterval, _cancellationTokenSource.Token);
            }
        }
    }
}