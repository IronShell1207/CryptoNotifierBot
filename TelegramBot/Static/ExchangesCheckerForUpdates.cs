using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Static;

namespace TelegramBot.Static
{
    public class ExchangesCheckerForUpdates
    {
        public static SymbolTimedExInfo binancePairsData { get; set; }
        public static SymbolTimedExInfo gateioPairsData { get; set; }
        public static SymbolTimedExInfo okxPairsData { get; set; }
        public static SymbolTimedExInfo kucoinPairsData { get; set; }
        public static List<List<SymbolTimedExInfo>> marketData { get; set; } = new List<List<SymbolTimedExInfo>>();
        public static bool UpdaterAlive { get; set; } = false;

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
                marketData.Add(list);
                UpdaterAlive = true;
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

        public static async Task<double> GetCurrentPrice(TradingPair pair, string exchange)
        {
            var mData = marketData.LastOrDefault();
            SymbolTimedExInfo sData = mData.FirstOrDefault(x => x.Exchange == exchange);
            var pricedPair = sData.Pairs.FirstOrDefault(x => x.Symbol.ToString() == pair.ToString());
            if (pricedPair != null)
                return (double) pricedPair.Price;
            return 0;
        }

        public static async Task<List<string>> GetExchangesForPair(TradingPair pair)
        {
            List<string> exchanges = new List<string>();
            try
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
            catch (NullReferenceException ex)
            {
                while (!UpdaterAlive)
                {
                    Thread.Sleep(300);
                }

                return GetExchangesForPair(pair).Result;
            }

        }
    }
}
