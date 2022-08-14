using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        private static List<IntervaledUsersHistory> _monUsersUpdate = new();
        public static bool MonitorLoopCancellationToken { get; set; } = true;

        private static async void CrtMsg(List<(CryptoPair, double)> lst, StringBuilder sb, UserConfig user,
            string msg = "")
        {
            if (lst.Any())
            {
                foreach (var pair in lst)
                    sb.AppendLine(FormatNotifyEntryStock(pair.Item1, pair.Item2));
                await BotApi.SendMessage(user.TelegramId, msg + sb);
            }
        }

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
                        var lastMsg = lastUpdateUsers?.LastOrDefault(x => x.UserId == user.Id)?.LastMsgId;
                        var pairsDefault = await UserTasksToNotify(user, dbContext, true);
                        var pairsSingleNotify = await UserTasksSingleNotify(user, dbContext);
                        var pairsTriggeredButRaised = await UserTriggeredTasksRaised(user, dbContext);
                        var pairMon = await UserTasksMon(user, dbContext);
                        //if (pairsDefault.Any())
                        //{
                        //    foreach (var pair in pairsDefault)
                        //        sb.AppendLine(FormatNotifyEntryStock(pair.Item1, pair.Item2));
                        //    if (user.RemoveLatestNotifyBeforeNew && lastMsg != null)
                        //        await BotApi.RemoveMessage(user.TelegramId, (int)lastMsg);
                        //    var msg = await BotApi.SendMessage(user.TelegramId, sb.ToString());
                        //    var tpl = new IntervaledUsersHistory(user.Id, DateTime.Now, msg?.MessageId);
                        //    lastUpdateUsers.Add(tpl);
                        //}
                        CrtMsg(pairsSingleNotify, sb, user);
                        CrtMsgMoon(pairMon, user);
                        CrtMsg(pairsTriggeredButRaised, sb, user,
                            $"⚠️Pairs triggered, but raise above or fall bellow trigger again:\n");
                    }
                }

                Thread.Sleep(420);
            }
        }

        public static async Task<List<(MonObj, double)>> UserTasksMon(UserConfig user, AppDbContext dbContext)
        {
            List<(MonObj, double)> tasks = new List<(MonObj, double)>();
            DateTime dateTimenow = DateTime.Now.ToUniversalTime().AddHours(user.TimezoneChange);
            var lastMsg = _monUsersUpdate?.LastOrDefault(x => x.UserId == user.Id);
            if (lastMsg == null)
            {
                lastMsg = new IntervaledUsersHistory(user.Id, dateTimenow, user.TelegramId);
                _monUsersUpdate?.Add(lastMsg);
            }

            if (!(user.NightModeEnable && !NightTime(user.NightModeStartTime, user.NightModeEndsTime, dateTimenow)))
            {
                if (lastMsg.LastUpdateDateTime < dateTimenow.AddMinutes(-1))
                {
                    var pairs = dbContext.MonPairs.Where(x => x.OwnerId == user.Id);
                    foreach (var pair in pairs.ToList())
                    {
                        var price = await Program.cryptoData.GetCurrentPricePairByName(pair.PairBase.ToUpper(), "USDT");
                        if (price?.Price > 0)
                            tasks.Add(new(pair, price.Price));
                    }
                    if (tasks.Count > 0)
                    {
                        lastMsg.LastUpdateDateTime = dateTimenow.AddMinutes(1);
                    }
                }
            }
            return tasks;
        }

        public static async Task<List<(CryptoPair, double)>> UserTasksToNotify(UserConfig user, AppDbContext dbContext,
                bool useInterval = true)
        {
            List<(CryptoPair, double)> tasks = new List<(CryptoPair, double)>();
            DateTime dateTimenow = DateTime.Now.AddHours(user.TimezoneChange - 3);

           /* if (!useInterval || UpdateIntervalExpired(user.Id, user.NoticationsInterval) && !(user.NightModeEnable &&
                    !NightTime(user.NightModeStartTime, user.NightModeEndsTime, dateTimenow)))
            {
                //var pairs = dbContext.CryptoPairs.Where(x => x.OwnerId == user.Id && x.Enabled && !x.TriggerOnce).ToList();
                foreach (var pair in user.pairs.Where(x => x.OwnerId == user.Id && x.Enabled && !x.TriggerOnce)
                             .ToList())
                {
                    var price = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());
                    if (price?.Price > 0 && (price.Price > pair.Price && pair.GainOrFall ||
                                             price.Price < pair.Price && !pair.GainOrFall))
                        tasks.Add(new(pair, price.Price));
                }
            }*/

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
                    if (price?.Price > 0 && (price.Price > pair.Price && pair.GainOrFall ||
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

        public static async Task<List<(CryptoPair, double)>> UserTriggeredTasksRaised(UserConfig user,
            AppDbContext dbContext)
        {
            List<(CryptoPair, double)> tasksReturing = new List<(CryptoPair, double)>();
            var pairsTriggered = user?.pairs?.Where(x => x.TriggerOnce && x.Triggered)?.ToList();
            if (pairsTriggered?.Count > 0)
            {
                foreach (var pair in pairsTriggered)
                {
                    var price = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());
                    if (price?.Price > 0 && ((price.Price * 1.01) < pair.Price && pair.GainOrFall ||
                                             (price.Price * 0.99) > pair.Price && !pair.GainOrFall))
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
            var gorfall = pair.Price < newprice;
            var enabledSymbol = gorfall ? "▲" : "▼";
            var gainOrFallSymbol = gorfall ? "raise 📈" : "fall 📉";
            var priceDiff = gorfall ? ((newprice / pair.Price) * 100) - 100 : ((newprice / pair.Price) * 100) - 100;
            var plusic = gorfall ? "+" : "";
            return
                $"{enabledSymbol} {pair.Id} {pair.PairBase}/{pair.PairQuote} {plusic}{string.Format("{0:##0.00#}", priceDiff)}% {gainOrFallSymbol} {pair.Price}->{newprice}";
        }

        private static async void CrtMsgMoon(List<(MonObj, double)> lst, UserConfig user)
        {
            StringBuilder sb = new StringBuilder();

            if (lst.Any())
            {
                var lastMsg = _monUsersUpdate?.FirstOrDefault(x => x.UserId == user.Id);
                if (lastMsg != null && lastMsg.LastMsgId != null)
                {
                    
                    await BotApi.RemoveMessage(lastMsg.TelegramId, (int)lastMsg.LastMsgId);
                }
                foreach (var pair in lst)
                    sb.AppendLine(FormatStrStock(pair.Item1, pair.Item2));
                var msg = await BotApi.SendMessage(user.TelegramId, sb.ToString());
                lastMsg.LastMsgId = msg.MessageId;
            }
        }
        private static string FormatStrStock(MonObj pair, double price)
        {
            return $"{pair.PairBase} - {price}";
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


        private static bool NightTime(TimeSpan start, TimeSpan end, DateTime now)
        {
            TimeSpan timeNow = now.TimeOfDay;
            CultureInfo provider = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = provider;
            System.Globalization.DateTimeStyles style = DateTimeStyles.None;
            if (DateTime.Now.TimeOfDay.IsBetween(start, end))
            {
                return true;
            }
            /*var daysStart = start > (now.Hour * 60 + now.Minute) && (now.Hour * 60 + now.Minute) < end ? -1 : 0;
            var dateStart = new DateTime(now.Year, now.Month, now.Day + daysStart, startHours, start - (startHours * 60), 0);
            int endHours = end / 60;
            int endDay = (now.Hour * 60 + now.Minute) > end && (now.Hour * 60 + now.Minute) < start ? 1 : 0;
            var dateEnd = new DateTime(now.Year, now.Month, now.Day, endHours, end - (endHours * 60), 0).AddDays(endDay);
            if (now > dateStart && now < dateEnd)
                return true;
            return false;*/
            return false;
        }

    }
    static class TimeSpanExtensions
    { 
        public static bool IsBetween(this TimeSpan time,
            TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime == startTime)
            {
                return true;
            }

            if (endTime < startTime)
            {
                return time <= endTime ||
                       time >= startTime;
            }

            return time >= startTime &&
                   time <= endTime;
        }
    }
    public class IntervaledUsersHistory
    {
        public int UserId { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public long TelegramId { get; set; }
        public int? LastMsgId { get; set; }
        public IntervaledUsersHistory(int userid, DateTime datetime, long telegramId = 0)
        {
            UserId = userid;
            LastUpdateDateTime = datetime;
            if (telegramId != 0)
                TelegramId = telegramId;
        }

        public IntervaledUsersHistory(int userid, DateTime datetime, int? msg, long telegramId = 0)
        {

            UserId = userid;
            LastUpdateDateTime = datetime;
            LastMsgId = msg;
            if (telegramId!= 0)
            TelegramId = telegramId;
        }
    }
}
