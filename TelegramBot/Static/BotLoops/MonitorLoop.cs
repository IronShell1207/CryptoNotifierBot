using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Telegram.Bot.Types;
using TelegramBot.Objects;

namespace TelegramBot.Static.BotLoops
{
    public class MonitorLoop
    {
        private static List<IntervaledUsersHistory> lastUpdateUsers = new List<IntervaledUsersHistory>() { };
        public static bool MonitorLoopCancellationToken { get; set; } = true;
        public static async void Loop()
        {
            while (MonitorLoopCancellationToken)
            {

                using (AppDbContext dbContext = new AppDbContext())
                {
                    foreach (UserConfig user in dbContext.Users)
                    {
                        StringBuilder sb = new StringBuilder();
                        List<(CryptoPair, double)> pairs = await UserTasksToNotify(user, dbContext, true);
                        if (pairs.Any())
                        {
                            foreach (var pair in pairs)
                                sb.AppendLine(FormatNotifyEntryStock(pair.Item1, pair.Item2));

                            lastUpdateUsers.Add(new IntervaledUsersHistory(user.Id, DateTime.Now));
                            await BotApi.SendMessage(user.TelegramId, sb.ToString());

                        }
                    }
                }
            }
            Thread.Sleep(1420);
        }

        public static async Task<List<(CryptoPair, double)>> UserTasksToNotify(UserConfig user, AppDbContext dbContext, bool useInterval = true)
        {
            List<(CryptoPair, double)> tasks = new List<(CryptoPair, double)>();
            DateTime dateTimenow = DateTime.Now;
            if (!useInterval || UpdateIntervalExpired(user.Id, user.NoticationsInterval) && !(user.NightModeEnable && !NightTime(
                    user.NightModeStartTime, user.NightModeEndsTime,
                    dateTimenow.Hour * 60 + dateTimenow.Minute)))
            {
                var pairs = dbContext.CryptoPairs.Where(x => x.OwnerId == user.Id && x.Enabled).ToList();
                foreach (var pair in pairs)
                {
                    ////var price = await ExchangesCheckerForUpdates.GetCurrentPrice(
                    ////    new TradingPair(pair.PairBase, pair.PairQuote), pair.ExchangePlatform);
                    //if (price>0 &&( price > pair.Price && pair.GainOrFall || price < pair.Price && !pair.GainOrFall))
                    //{
                    //    tasks.Add(new(pair, price));
                    //}
                }
            }
            return tasks;
        }

        private static string FormatNotifyEntryStock(CryptoPair pair, double newprice)
        {
            var enabledSymbol = pair.GainOrFall ? "▲" : "▼";
            var gainOrFallSymbol = pair.GainOrFall ? "raise 📈" : "fall 📉";
            var priceDiff = pair.GainOrFall ? ((newprice / pair.Price) * 100) - 100 : ((newprice / pair.Price) * 100) - 100;
            var plusic = pair.GainOrFall ? "+" : "";
            return $"{enabledSymbol} {pair.Id} {pair.PairBase}/{pair.PairQuote} {plusic}{string.Format("{0:##0.00#}", priceDiff)}% {gainOrFallSymbol} {pair.Price}->{newprice}";
        }
        // Пока не доделываю (нужно разобраться с gain и fall price, мб сделать отдельные формации под них)
        private static string FormatNotifyEntryByUserFormat(string formater, CryptoPair pair, double newPrice)
        {
            var enabledSymbol = pair.Enabled ? "✅" : "🛑";
            return null;
        }
        private static bool UpdateIntervalExpired(int userId, int interval)
        {
            var lastupdated = lastUpdateUsers.FirstOrDefault(x => x.UserId == userId);
            if (lastupdated == null) return true;
            var nowDate = DateTime.Now;
            if (lastupdated.LastUpdateDateTime + TimeSpan.FromSeconds(interval) < nowDate)
            {
                lastUpdateUsers.Remove(lastupdated);
                return true;
            }

            return false;
        }
        private static bool NightTime(int start, int end, int now)
        {
            if (start > now && now > end) return false;
            return true;
        }
    }

    public class IntervaledUsersHistory
    {
        public int UserId { get; set; }
        public DateTime LastUpdateDateTime { get; set; }

        public IntervaledUsersHistory(int userid, DateTime datetime)
        {
            UserId = userid;
            LastUpdateDateTime = datetime;
        }
    }
}
