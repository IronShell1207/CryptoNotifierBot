using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class MorningReport
    {
        private static List<UserConfig> NotifiedUsers = new List<UserConfig>();
        public static bool MorningPool { get; set; } = true;
        private static List<TradingPair> MorningPairs = new List<TradingPair>()
        {
            new("BTC","USDT"), new("ETC", "USDT"),
            new("ETH", "USDT"), new("XMR", "USDT")
        };
        public static void MorningSpread()
        {
            while (MorningPool)
            {
                var dateNow = DateTime.Now;
                if (dateNow.Hour == 0 && dateNow.Minute == 0 && 0 < dateNow.Second & dateNow.Second < 59)
                {
                    NotifiedUsers.Clear();
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                    Console.WriteLine($"[{dateNow.ToString()}] Morning report notified users list cleared");
                }
                var users = new AppDbContext().Users.Where(x => x.MorningReport != null).ToList();
                foreach (var userCfg in users)
                {
                    if (NotifiedUsers.Contains(userCfg))
                        users.Remove(userCfg);
                }

                foreach (var user in users)
                {
                    var timingHours = Math.Round((decimal)user.MorningReport / 60);
                    var timingMins = user.MorningReport - (timingHours * 60);
                    StringBuilder mReport = new StringBuilder();
                    mReport.AppendLine("Morning report. Prices changed by 8 and 24 hours:");
                    if (timingHours == dateNow.Hour && (timingMins - 1 < dateNow.Minute && dateNow.Minute < timingMins + 1))
                    {
                        //var pricesNow = ExchangesCheckerForUpdates.MarketData.LastOrDefault();
                        //var prices8hAgo = ExchangesCheckerForUpdates.MarketData.Where(
                        //    x => x[0].CreationTime + TimeSpan.FromMinutes(((7 * 60) + 58)) < dateNow &&
                        //         dateNow < x[0].CreationTime + TimeSpan.FromMinutes(((8 * 60) + 1))).FirstOrDefault();
                        //var prices24hAgo = ExchangesCheckerForUpdates.MarketData.Where(
                        //    x => x[0].CreationTime + TimeSpan.FromMinutes(((23 * 60) + 58)) < dateNow &&
                        //         dateNow < x[0].CreationTime + TimeSpan.FromMinutes(((24 * 60) + 1))).FirstOrDefault();
                       
                        //foreach (TradingPair pair in MorningPairs)
                        //{
                        //    var priceNow = pricesNow[0].Pairs.First(x => x.Symbol == pair);
                        //    var price8hAge = prices8hAgo?[0].Pairs.First(x => x.Symbol == pair);
                        //    var price24hAge = prices24hAgo?[0].Pairs.First(x => x.Symbol == pair);
                        //    var Trend8H = priceNow.Price > price8hAge.Price ? "📈 raises " : "📉 falls";
                        //    var Trend24H = priceNow.Price > price24hAge.Price ? "📈 raises" : "📉 falls";
                        //    if (price8hAge != null)
                        //        mReport.AppendLine(
                        //            $"{pair.ToString()} {Trend8H} from {price8hAge.Price}->{priceNow.Price} in 8H");
                        //    if (price24hAge != null)
                        //        mReport.AppendLine(
                        //            $"{pair.ToString()} {Trend24H} from {price24hAge.Price}->{priceNow.Price} in 24H");
                        //}
                        //NotifiedUsers.Add(user);
                        //BotApi.SendMessage(user.TelegramId, mReport.ToString());
                    }
                    
                }
                Thread.Sleep(TimeSpan.FromSeconds(25));

            }
        }
    }
}
