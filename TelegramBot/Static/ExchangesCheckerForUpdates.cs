using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using CryptoApi.Static;
using Microsoft.EntityFrameworkCore.Storage;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class ExchangesCheckerForUpdates
    {
        public static bool DataAvailable { get; set; } = false;
        public static bool DataUpdating { get; set; } = true;
        public static SymbolTimedExInfo binancePairsData { get; set; }
        public static SymbolTimedExInfo gateioPairsData { get; set; }
        public static SymbolTimedExInfo okxPairsData { get; set; }
        public static SymbolTimedExInfo kucoinPairsData { get; set; }
        public static SymbolTimedExInfo bitgetPairsData { get; set; }

        private static List<List<SymbolTimedExInfo>> _marketData = new List<List<SymbolTimedExInfo>>();
        public static List<List<SymbolTimedExInfo>> MarketData
        {
            get
            {
                return _marketData;
            }
            set
            {
                _marketData = value;
            }
        }

        public static bool Updating { get; set; } = false;

        public static async void ExchangesUpdaterLoop()
        {
            var binanceApi = new BinanceApi();
            var gateIOApi = new GateioApi();
            var okxApi = new OkxApi();
            var kucoinApi = new KucoinAPI();
            var bitgetApi = new BitgetApi();
            while (DataUpdating)
            {
                try
                {
                    binancePairsData = await binanceApi.GetExchangeData();
                    gateioPairsData = await gateIOApi.GetExchangeData();
                    okxPairsData = await okxApi.GetExchangeData();
                    kucoinPairsData = await kucoinApi.GetExchangeData();
                    bitgetPairsData = await bitgetApi.GetExchangeData();
                    var list = new List<SymbolTimedExInfo>()
                    {binancePairsData, gateioPairsData, okxPairsData, kucoinPairsData, bitgetPairsData};
                    MarketData.Add(list);
                    Console.WriteLine($"[{DateTime.Now.ToString()}] Market data updated. BTC: {binancePairsData.Pairs.FirstOrDefault(x=>x.Symbol.ToString()== "BTC/USDT").Price}$ Binance: {binancePairsData.Pairs.Count} GateIO: {gateioPairsData.Pairs.Count} Okx: {okxPairsData.Pairs.Count} Kucoin: {kucoinPairsData.Pairs.Count} Bitget: {bitgetPairsData.Pairs.Count}");
                    DataAvailable = true;
                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public static async Task<double> GetCurrentPrice(TradingPair pair, string exchange = "")
        {
            if (DataAvailable)
            {
                var mData = MarketData.LastOrDefault();
                if (string.IsNullOrWhiteSpace(exchange))
                    exchange = GetExchangesForPair(pair).Result.FirstOrDefault();
                SymbolTimedExInfo sData = mData.FirstOrDefault(x => x.Exchange == exchange);
                var pricedPair = sData.Pairs.FirstOrDefault(x => x.Symbol.ToString() == pair.ToString());
                if (pricedPair != null)
                    return (double)pricedPair.Price;
            }
            else
            {
                Thread.Sleep(2000);
                return GetCurrentPrice(pair, exchange).Result;
            }
            return 0;

        }

       
        public static async Task<List<string>> GetExchangesForPair(TradingPair pair)
        {
            List<string> exchanges = new List<string>();

            try
            {
                if (DataAvailable)
                {
                    if (binancePairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.Binance);
                    if (gateioPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.GateIO);
                    if (okxPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.Okx);
                    if (kucoinPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.Kucoin);
                    if (bitgetPairsData.Pairs.Exists(x=>x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.Bitget);
                    return exchanges;
                }
                else
                {
                    Thread.Sleep(2000);
                    return GetExchangesForPair(pair).Result;
                }
            }
            catch (NullReferenceException ex)
            {

                return GetExchangesForPair(pair).Result;
            }

            return GetExchangesForPair(pair).Result;
        }
    }
}
