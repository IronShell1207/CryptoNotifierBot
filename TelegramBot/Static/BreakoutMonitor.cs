using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class BreakoutMonitor
    {
        private static DateTime br5MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br2MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br15MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br30MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br45MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br60MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br120MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br240MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br480MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br960MinLastDateTime { get; set; } = DateTime.Now;
        private static DateTime br1920MinLastDateTime { get; set; } = DateTime.Now;
        private static List<int> ListTimings = new List<int>() { 2, 5, 15, 30, 45, 60, 120, 240, 480, 960, 1920 };

        private static List<DateTime> listDateTimes = new List<DateTime>()
        {   br2MinLastDateTime,
            br5MinLastDateTime,br15MinLastDateTime, br30MinLastDateTime, br45MinLastDateTime, br60MinLastDateTime,
            br120MinLastDateTime,br240MinLastDateTime, br480MinLastDateTime, br960MinLastDateTime, br1920MinLastDateTime
        };

        private static List<double> procentsDifference = new List<double>() { 2.5, 3, 4, 5, 5, 5, 5, 5, 6, 6, 8 };
        public static async void BreakoutLoop()
        {
            var count = ExchangesCheckerForUpdates.marketData.Count();
            while (true)
            {
                if (count > 3000)
                    ExchangesCheckerForUpdates.marketData.RemoveRange(1500, 500);
                var countNow = ExchangesCheckerForUpdates.marketData.Count();
                if (countNow <= count)
                {
                    goto loopend;
                }
                var datetimeNow = DateTime.Now;
                var latestData = ExchangesCheckerForUpdates.marketData[0];
                for (var index = ExchangesCheckerForUpdates.marketData.Count - 1; index > 0; index--)
                {
                    List<SymbolTimedExInfo> data = ExchangesCheckerForUpdates.marketData[index];
                    for (var iTi = 0; iTi < ListTimings.Count; iTi++)
                    {
                        int timeIn = ListTimings[iTi];
                        if (data[0].CreationTime + TimeSpan.FromMinutes(timeIn) < datetimeNow &&
                            datetimeNow < data[0].CreationTime + TimeSpan.FromMinutes(timeIn + 1) && 
                            listDateTimes[iTi] + TimeSpan.FromMinutes(timeIn) < datetimeNow)
                        {
                            listDateTimes[iTi] = datetimeNow;
                            index += timeIn + 1;
                            for (int i = 0; i < data.Count; i++)
                            {
                                SymbolTimedExInfo dataExchange = data[i];
                                var compairedPairs = CompairedPairs(dataExchange.Pairs, latestData[i].Pairs,
                                    procentsDifference[iTi], ListTimings[iTi], dataExchange.Exchange);
                                Console.WriteLine($"[{datetimeNow.ToString()}] {data[i].Exchange}: {compairedPairs.Count}");
                                if (compairedPairs.Count > 0)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append($"Updated data from {dataExchange.Exchange}:\n");
                                    foreach (var pair in compairedPairs)
                                    {
                                        var riimg = pair.oldPrice > pair.newPrice ? "📉" : "📈";
                                        var pls = pair.oldPrice > pair.newPrice ? "" : "+";
                                        var formater = pair.newPrice < 0.0099 ? "{0:0.#########}" : "{0:####0.###}";
                                        sb.Append(
                                            $"{riimg} {pair.Symbol.ToString()} {string.Format(formater, pair.oldPrice)}->{string.Format(formater, pair.newPrice)} {pls}{string.Format("{0:##.00}", ((pair.newPrice / pair.oldPrice) * 100) - 100)}% in {pair.Time} mins\n");
                                    }

                                    SpreadBreakoutNotify(sb.ToString(), data[i].Exchange, 2);
                                    Thread.Sleep(25);
                                }
                            }
                        }
                    }
                }
                loopend:
                count = countNow;
                Thread.Sleep(30000);
            }


        }

        public static async void SpreadBreakoutNotify(string msg, string platform, int timing)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] Breakout bot sending notify");
            using (AppDbContext db = new AppDbContext())
            {
                foreach (BreakoutSub sub in db.BreakoutSubs.ToList())
                {
                    if ((timing == 2 && sub.S2MinUpdates)
                        || (timing == 5 && sub.S5MinUpdates)
                        || (timing == 15 && sub.S15MinUpdates)
                        || (timing == 30 && sub.S30MinUpdates)
                        || (timing == 45 && sub.S45MinUpdates)
                        || (timing == 60 && sub.S60MinUpdates)
                        || (timing == 120 && sub.S120MinUpdates)
                        || (timing == 240 && sub.S240MinUpdates)
                        || (timing == 480 && sub.S480MinUpdates)
                        || (timing == 960 && sub.S960MinUpdates)
                        || (timing == 1920 && sub.S1920MinUpdates))
                    {
                        //StringBuilder sendingMessage = new StringBuilder();
                        if ((platform == Exchanges.Binance && sub.BinanceSub)
                            || (platform == Exchanges.Kucoin && sub.KucoinSub)
                            || (platform == Exchanges.Okx && sub.OkxSub)
                            || (platform == Exchanges.GateIO && sub.GateioSub))
                        {
                            await BotApi.SendMessage(sub.TelegramId, msg);
                        }
                    }
                }
            }
        }
        private static List<BreakuotPair> CompairedPairs(List<CryptoExchangePairInfo> oldData, List<CryptoExchangePairInfo> freshData, double procent, double time = 0, string exchange = "Binance")
        {
            List<BreakuotPair> changedData = new List<BreakuotPair>() { };
            foreach (var data in oldData)
            {
                var fresh = freshData.FirstOrDefault(x => x.Symbol.ToString() == data.Symbol.ToString());
                if (fresh == null || fresh.Symbol.ToString() == "/") break;
                var diff = ((data.Price / fresh.Price) * 100) - 100;
                var proc = data.Price < 0.0000009 ? 6 : procent;
                if (diff > proc || diff < -proc)
                    changedData.Add(new BreakuotPair()
                    {
                        newPrice = fresh.Price,
                        oldPrice = data.Price,
                        Symbol = fresh.Symbol,
                        Time = time,
                        Exchange = exchange
                    });
                //var grp = diff > 0 ? "📈" : "📉";
                //string compairedStr = $"{grp} {data.Symbol.ToString()} {diff} 1.60500 => 1.64300  0.77 m!";
            }

            return changedData;
        }
    }
}
