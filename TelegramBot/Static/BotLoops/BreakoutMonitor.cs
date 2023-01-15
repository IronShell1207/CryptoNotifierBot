using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Static.DataHandler;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    /*public class BreakoutMonitor
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
        private static List<int> ListTimings = new() { 3, 5, 15, 30, 45, 60, 120, 240, 480, 960, 1920 };

        private static List<DateTime> listDateTimes = new()
        {
            br2MinLastDateTime,
            br5MinLastDateTime,
            br15MinLastDateTime,
            br30MinLastDateTime,
            br45MinLastDateTime,
            br60MinLastDateTime,
            br120MinLastDateTime,
            br240MinLastDateTime,
            br480MinLastDateTime,
            br960MinLastDateTime,
            br1920MinLastDateTime
        };

        private static List<double> procentsDifference = new() { 2.5, 3, 4, 5, 5, 5, 5, 5, 6, 6, 8 };

        public static async void BreakoutLoop()
        {
            while (true)
            {
                var datetimeNow = DateTime.Now;
                StringBuilder sb = new StringBuilder();
                var latestData = await Program.cryptoData.GetLatestDataSets();
                for (var indexTiming = 0; indexTiming < ListTimings.Count; indexTiming++)
                {
                    int timing = ListTimings[indexTiming];
                    if (listDateTimes[indexTiming] + TimeSpan.FromMinutes(timing) < datetimeNow)
                    {
                        var oldData = await Program.cryptoData.GetLatestDataSets(timing);
                        if (oldData != null)
                            for (int i = 0; i < oldData.Count; i++)
                            {
                                var oldExchangeData = oldData[i];
                                var freshExchangeData =
                                    latestData.FirstOrDefault(x => x.Exchange == oldExchangeData.Exchange);
                                if (freshExchangeData != null)
                                {
                                    var compairedPairs =
                                        CompairedPairs(oldExchangeData, freshExchangeData,
                                            procentsDifference[indexTiming],
                                            timing, oldExchangeData.Exchange);
                                    if (compairedPairs.Any())
                                    {
                                        listDateTimes[indexTiming] = datetimeNow;
                                        sb.Append($"{oldData[i].Exchange}: {compairedPairs.Count} Time: {timing} ");
                                        SpreadBreakoutNotify(compairedPairs, oldExchangeData.Exchange, timing);
                                    }
                                }
                            }
                    }
                }
                if (sb.Length > 0)
                    Console.WriteLine("Breakouts: " + sb.ToString());
                Thread.Sleep(10000);
            }
        }

        public static async void SpreadBreakoutNotify(List<BreakoutPair> pairs, string platform, int timing)
        {
            //ConsoleCommandsHandler.LogWrite($"Breakout bot sending notify");
            await using (AppDbContext db = new AppDbContext())
                foreach (BreakoutSub sub in db.BreakoutSubs.ToList())
                    if (sub.Subscribed)
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
                            //StringBuilder sendingMessage = new StringBuilder();
                            if ((platform == Exchanges.Binance && sub.BinanceSub)
                                || (platform == Exchanges.Kucoin && sub.KucoinSub)
                                || (platform == Exchanges.Okx && sub.OkxSub)
                                || (platform == Exchanges.GateIO && sub.GateioSub)
                                || (platform == Exchanges.Bitget && sub.BitgetSub))
                            {
                                //ConsoleCommandsHandler.LogWrite($"{sub.TelegramId}");
                                StringBuilder sb = new StringBuilder($"Updated data from {platform}:\n");
                                var blackList = db.BlackListedPairs.Where(x => x.OwnerId == sub.Id).ToList();
                                if (!sub.WhitelistInsteadBlack && blackList.Any() && sub.BlackListEnable)
                                {
                                    var newList = pairs;
                                    foreach (var pair in blackList)
                                    {
                                        newList.RemoveAll(x => x.Symbol.ToString() == pair.ToString());
                                    }
                                    if (newList.Any())
                                        FormatAndSendAsync(newList, platform, timing, sb, sub);
                                }
                                else if (sub.WhitelistInsteadBlack && blackList.Any() && sub.BlackListEnable)
                                {
                                    var newList = new List<BreakoutPair>();
                                    foreach (var pair in blackList)
                                    {
                                        var bpr = pairs.FirstOrDefault(x => x.Symbol.ToString() == pair.ToString());
                                        if (bpr != null) newList.Add(bpr);
                                    }
                                    if (newList.Any())
                                        FormatAndSendAsync(newList, platform, timing, sb, sub);
                                }
                                else FormatAndSendAsync(pairs, platform, timing, sb, sub);
                            }
        }

        private static async void FormatAndSendAsync(List<BreakoutPair> pairs, string platform, int timing, StringBuilder sb, BreakoutSub sub)
        {
            foreach (var pair in pairs)
            {
                //if (favList?.Contains(pair))
                var riimg = pair.oldPrice > pair.newPrice ? "📉" : "📈";
                var pls = pair.oldPrice > pair.newPrice ? "" : "+";
                var formater = pair.newPrice < 0.0099 ? "{0:0.#########}" : "{0:####0.####}";
                sb.Append(
                    $"{riimg} {pair.Symbol.ToString()} {string.Format(formater, pair.oldPrice)}->{string.Format(formater, pair.newPrice)} {pls}{string.Format("{0:##.00}", ((pair.newPrice / pair.oldPrice) * 100) - 100)}% in {pair.Time} mins\n");
            }
            await BotApi.SendMessage(sub.TelegramId, sb.ToString());
        }

        private static List<BreakoutPair> CompairedPairs(CryDbSet oldExchangeData, CryDbSet freshExchangeData, double procent, double time = 0, string exchange = "Binance")
        {
            List<BreakoutPair> changedData = new List<BreakoutPair>() { };
            var oldPairs = oldExchangeData.pairs;
            var freshPairs = freshExchangeData.pairs;
            for (int pairIndex = 0; pairIndex < oldPairs.Count; pairIndex++)
            {
                var oldPairData = oldPairs[pairIndex];
                var freshPairData = freshPairs.FirstOrDefault(x => x.ToString() == oldPairData.ToString());
                if (freshPairData != null)
                {
                    var diff = ((oldPairData.Price / freshPairData.Price) * 100) - 100;
                    var proc = oldPairData.Price < 0.0000009 ? 6 : procent;
                    if (diff > proc || diff < -proc)
                        changedData.Add(new BreakoutPair()
                        {
                            newPrice = freshPairData.Price,
                            oldPrice = oldPairData.Price,
                            Symbol = freshPairData,
                            Time = time,
                            Exchange = exchange
                        });
                    //var grp = diff > 0 ? "📈" : "📉";
                    //string compairedStr = $"{grp} {data.Symbol.ToString()} {diff} 1.60500 => 1.64300  0.77 m!";
                }
            }

            return changedData;
        }*/
}