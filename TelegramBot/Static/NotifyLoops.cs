using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CryptoApi;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Static;
using TelegramBot.Objects;
using CryptoPair = TelegramBot.Objects.CryptoPair;

namespace TelegramBot.Static
{
    public class NotifyLoops
    {

        public static async void MainLoop()
        {
            Task.Run(() => ExchangesCheckerForUpdates.ExchangesUpdaterLoop());
            Task.Run(() => BreakoutMonitor.BreakoutLoop());
            while (true)
            {
                if (!ExchangesCheckerForUpdates.UpdaterAlive) // ( ExchangesCheckerForUpdates.binancePairsData == null || ExchangesCheckerForUpdates.gateioPairsData == null || ExchangesCheckerForUpdates.okxPairsData == null ||
                                                              // ExchangesCheckerForUpdates.kucoinPairsData == null)
                {
                    Thread.Sleep(2000);
                }
                else
                {
                    using (AppDbContext dbContext = new AppDbContext())
                    {
                        var pairsList = dbContext.CryptoPairs.Count() > 0 ? dbContext.CryptoPairs.ToList() : new List<CryptoPair>() { };
                        foreach (var pair in pairsList)
                        {
                            CryptoExchangePairInfo pairPriced;
                            switch (pair.ExchangePlatform)
                            {
                                case "Binance":
                                    pairPriced = ExchangesCheckerForUpdates.binancePairsData.Pairs.Where(x => x.Symbol.ToString() == pair.PairBase.ToString()).FirstOrDefault();
                                    break;
                                    // case "GateIO":
                            }
                        }

                    }
                }

                Thread.Sleep(1420);
            }
        }

        
        


    }
}
