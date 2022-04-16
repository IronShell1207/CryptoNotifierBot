using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Static;
using Microsoft.EntityFrameworkCore.Storage;

namespace TelegramBot.Static
{
    public class ExchangesCheckerForUpdates
    {
        public static SymbolTimedExInfo binancePairsData { get; set; }
        public static SymbolTimedExInfo gateioPairsData { get; set; }
        public static SymbolTimedExInfo okxPairsData { get; set; }
        public static SymbolTimedExInfo kucoinPairsData { get; set; }
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
            while (true)
            {
                //try
                // {

                binancePairsData = BinanceApi.GetExchangeData();
                gateioPairsData = GateioApi.GetExchangeData();
                okxPairsData = OkxApi.GetExchangeData();
                kucoinPairsData = KucoinAPI.GetExchangeData();
                var list = new List<SymbolTimedExInfo>()
                    {binancePairsData, gateioPairsData, okxPairsData, kucoinPairsData};
                MarketData.Add(list);
                Console.WriteLine($"[{DateTime.Now.ToString()}] Market data updated. Binance: {binancePairsData.Pairs.Count} GateIO: {gateioPairsData.Pairs.Count} Okx: {okxPairsData.Pairs.Count} Kucoin: {kucoinPairsData.Pairs.Count}");
                Thread.Sleep(30000);
                //}
                //catch (Exception ex)
                // {
                //     UpdaterAlive = false;
                //  throw;
                //  }
            }
        }

        public static async Task<double> GetCurrentPrice(TradingPair pair, string exchange = "")
        {
            if (DataAvailable().Result)
            {
                var mData = MarketData.LastOrDefault();
                if (string.IsNullOrWhiteSpace(exchange))
                    exchange = GetExchangesForPair(pair).Result.FirstOrDefault();
                SymbolTimedExInfo sData = mData.FirstOrDefault(x => x.Exchange == exchange);
                var pricedPair = sData.Pairs.FirstOrDefault(x => x.Symbol.ToString() == pair.ToString());
                if (pricedPair != null)
                    return (double) pricedPair.Price;
            }
            else
            {
                Thread.Sleep(2000);
                return GetCurrentPrice(pair, exchange).Result;
            }
            return 0;
            
        }

        public async static Task<bool> DataAvailable()
        {
            if (binancePairsData == null || gateioPairsData == null || okxPairsData == null ||
                   kucoinPairsData == null)
            {
                return false;
            }

            return true;
        }
        public static async Task<List<string>> GetExchangesForPair(TradingPair pair)
        {
            List<string> exchanges = new List<string>();

            try
            {
                if (DataAvailable().Result)
                {
                    if (binancePairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.Binance);
                    if (gateioPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.GateIO);
                    if (okxPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.Okx);
                    if (kucoinPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                        exchanges.Add(CryptoApi.Constants.Exchanges.Kucoin);
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
