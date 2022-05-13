using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore;
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
                    var users = dbContext.Users.Include(x => x.pairs).OrderBy(x => x.Id).ToList();
                    foreach (UserConfig user in users)
                    {
                        StringBuilder sb = new StringBuilder();
                        var lastMsg = lastUpdateUsers.LastOrDefault(x => x.UserId == user.Id)?.LastMsgId;
                        var pairsDefault = await UserTasksToNotify(user, dbContext, true);
                        var pairsSingleNotify = await UserTasksSingleNotify(user, dbContext);
                        var pairsTriggeredButRaised = await UserTriggeredTasksRaised(user, dbContext);
                        if (pairsDefault.Any())
                        {
                            foreach (var pair in pairsDefault)
                                sb.AppendLine(FormatNotifyEntryStock(pair.Item1, pair.Item2));
                            if (user.RemoveLatestNotifyBeforeNew && lastMsg != null)
                                 await BotApi.RemoveMessage(user.TelegramId,(int)lastMsg);
                            var msg = await BotApi.SendMessage(user.TelegramId, sb.ToString());
                            var tpl = new IntervaledUsersHistory(user.Id, DateTime.Now, msg?.MessageId);
                            lastUpdateUsers.Add(tpl);
                        }

                        if (pairsSingleNotify.Any())
                        {
                            foreach (var pair in pairsSingleNotify)
                                sb.AppendLine(FormatNotifyEntryStock(pair.Item1, pair.Item2));
                            await BotApi.SendMessage(user.TelegramId, sb.ToString());
                        }

                        if (pairsTriggeredButRaised.Any())
                        {
                            foreach (var pair in pairsTriggeredButRaised)
                                sb.AppendLine(FormatNotifyEntryStock(pair.Item1, pair.Item2));
                            await BotApi.SendMessage(user.TelegramId, $"⚠️Pairs triggered, but raise above or fall bellow trigger again:\n{sb.ToString()}");
                        }
                    }
                }
                Thread.Sleep(420);
            }
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
                    var price = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());
                    if (price.Price > 0 &&(price.Price > pair.Price && pair.GainOrFall || 
                        price.Price < pair.Price && !pair.GainOrFall))
                        tasks.Add(new(pair, price.Price));
                }
            }
            return tasks;
        }

        public static async Task<List<(CryptoPair, double)>> UserTasksSingleNotify(UserConfig user,
            AppDbContext dbContext)
        {
            List<(CryptoPair, double)> tasksReturing = new List<(CryptoPair, double)>();
            var pairsSingle = user?.pairs?.Where(x => x.TriggerOnce && !x.Triggered)?.ToList();
           
            if (pairsSingle?.Count > 0)
            {
                foreach (var pair in pairsSingle)
                {
                    var price = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());
                    if (price.Price > 0 && (price.Price > pair.Price && pair.GainOrFall ||
                                            price.Price < pair.Price && !pair.GainOrFall))
                    {
                        tasksReturing.Add(new(pair, price.Price));
                        pair.Triggered = true;
                    }
                }

                dbContext.SaveChangesAsync();
            }

            return tasksReturing;
        }

        public static async Task<List<(CryptoPair, double)>> UserTriggeredTasksRaised(UserConfig user, AppDbContext dbContext)
        {
            List<(CryptoPair, double)> tasksReturing = new List<(CryptoPair, double)>();
            var pairsTriggered = user?.pairs?.Where(x => x.TriggerOnce && x.Triggered)?.ToList();
            if (pairsTriggered?.Count > 0)
            {
                foreach (var pair in pairsTriggered)
                {
                    var price = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());
                    if (price.Price > 0 && ((price.Price * 1.01) > pair.Price && pair.GainOrFall ||
                                            (price.Price * 0.99) < pair.Price && !pair.GainOrFall))
                    {
                        tasksReturing.Add(new(pair, price.Price));
                        pair.Triggered = false;
                    }
                }
                dbContext.SaveChangesAsync();
            }
            return tasksReturing;
        }

        private static string FormatNotifyEntryStock(CryptoPair pair, double newprice)
        {
            var enabledSymbol = pair.GainOrFall ? "▲" : "▼";
            var gainOrFallSymbol = pair.GainOrFall ? "raise 📈" : "fall 📉";
            var priceDiff = pair.GainOrFall ? ((newprice / pair.Price) * 100) - 100 : ((newprice / pair.Price) * 100) - 100;
            var plusic = pair.GainOrFall ? "+" : "";
            return $"{enabledSymbol} {pair.Id} {pair.PairBase}/{pair.PairQuote} {plusic}{string.Format("{0:##0.00#}", priceDiff)}% {gainOrFallSymbol} {pair.Price}->{newprice}";
        }
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
        public int? LastMsgId { get; set; }
        public IntervaledUsersHistory(int userid, DateTime datetime)
        {
            UserId = userid;
            LastUpdateDateTime = datetime;
        }

        public IntervaledUsersHistory(int userid, DateTime datetime, int? msg)
        {

            UserId = userid;
            LastUpdateDateTime = datetime;
            LastMsgId = msg;
        }
    }
}
