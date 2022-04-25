using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
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
                DateTime dateTimenow = DateTime.Now;
                using (AppDbContext dbContext = new AppDbContext())
                {
                    foreach (UserConfig user in dbContext.Users)
                    {
                        if (UpdateIntervalExpired(user.Id, user.NoticationsInterval) && !(user.NightModeEnable && !NightTime(user.NightModeStartTime, user.NightModeEndsTime,
                                dateTimenow.Hour * 60 + dateTimenow.Minute)))
                        {
                            var pairs = dbContext.CryptoPairs.Where(x => x.OwnerId == user.Id && x.Enabled).ToList();
                            StringBuilder sb = new StringBuilder();
                            foreach (var pair in pairs)
                            {
                                var price = await ExchangesCheckerForUpdates.GetCurrentPrice(
                                    new TradingPair(pair.PairBase, pair.PairQuote), pair.ExchangePlatform);
                                if (price > pair.Price && pair.GainOrFall || price < pair.Price && !pair.GainOrFall)
                                {
                                    //var formated = user.CryptoNotifyStyle != null ? user.CryptoNotifyStyle.Format(user.CryptoNotifyStyle, pair.PairBase=>"pBase", pair.PairQuote=>"pQuote" );
                                    sb.AppendLine(FormatNotifyEntryStock(pair, price));
                                }
                            }
                            if (sb.Length > 0)
                            {
                                lastUpdateUsers.Add(new IntervaledUsersHistory(user.Id, dateTimenow));
                                await BotApi.SendMessage(user.TelegramId, sb.ToString());
                            }
                        }
                    }
                }
                Thread.Sleep(1420);
            }
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
