using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.Objects;

namespace TelegramBot.Static.BotLoops
{
    /* public class MonitorLoop
     {
         private static List<UsersLastMessageIds> lastUpdateUsers = new();
         public static bool MonitorLoopCancellationToken { get; set; } = true;

         private static async Task<Message> CrtMsg(List<(CryptoPair, double)> lst, StringBuilder sb, UserConfig user,
             string msg = "", bool saveLastId = false)
         {
             if (lst.Any())
             {
                 foreach (var pair in lst)
                     sb.AppendLine(FormatNotifyEntryStock(pair.Item1, pair.Item2));
                 return await BotApi.SendMessage(user.TelegramId, msg + sb);
             }

             return null;
         }

         public static async void Loop()
         {
             while (MonitorLoopCancellationToken)
             {
                 Stopwatch timer = new Stopwatch();
                 timer.Start();
                 AppDbContext dbContext = new AppDbContext();

                 var users = dbContext.Users.Include(x => x.pairs).OrderBy(x => x.Id).ToList();
                 dbContext.Dispose();
                 var options = new ParallelOptions();
                 CancellationTokenSource ct = new CancellationTokenSource();

                 await Parallel.ForEachAsync(users, ct.Token, async (user, ct) =>
                 {
                     using (AppDbContext pContext = new AppDbContext())
                     {
                         if (ct.IsCancellationRequested) return;
                         StringBuilder sb = new StringBuilder();

                         var pairsDefault = await UserTasksToNotify(user, pContext, true);
                         var message = await CrtMsg(pairsDefault, new StringBuilder(), user);

                         if (message?.MessageId > 0)
                         {
                             var userMsgH = lastUpdateUsers.FirstOrDefault(x => x.User.TelegramId == user.TelegramId);
                             if (userMsgH == null)
                             {
                                 userMsgH = new UsersLastMessageIds(user);
                                 lastUpdateUsers.Add(userMsgH);
                             }

                             if (user.RemoveLatestNotifyBeforeNew && userMsgH.NormalMsg?.MsgId != null)
                                 await BotApi.RemoveMessage(user.TelegramId, (int)userMsgH.NormalMsg.MsgId);

                             userMsgH.NormalMsg = new SentMsg(DateTime.Now.ToUniversalTime(), message.MessageId);
                         }

                         var pairsSingleNotify = await UserTasksSingleNotify(user, pContext);
                         await CrtMsg(pairsSingleNotify, sb, user);

                         var pairsTriggeredButRaised = await UserTriggeredTasksRaised(user, pContext);
                         await CrtMsg(pairsTriggeredButRaised, sb, user,
                             $"⚠️Pairs triggered, but raise above or fall bellow trigger again:\n");

                         var pairMon = await UserTasksMon(user, pContext);
                         await CrtMsgMoon(pairMon, user);
                     }
                 });

                 timer.Stop();
                 //Console.WriteLine($"Data sended. Time elapsed: {timer.Elapsed} secs.");
                 Thread.Sleep(4000);
             }
         }

         public static async Task<List<(MonObj, double)>> UserTasksMon(UserConfig user, AppDbContext dbContext)
         {
             List<(MonObj, double)> tasks = new List<(MonObj, double)>();
             DateTime dateTimenow = DateTime.Now.ToUniversalTime().AddHours(user.TimezoneChange);

             if (!NightTime(user.NightModeEnable, user.NightModeStartTime, user.NightModeEndsTime, dateTimenow)
                 && UpdateMonIntervalExpired(user.Id, user.MonInterval))
             {
                 var pairs = dbContext.MonPairs.Where(x => x.OwnerId == user.Id);
                 foreach (var pair in pairs.ToList())
                 {
                     var price = await Program.cryptoData.GetCurrentPricePairByName(pair.PairBase.ToUpper(), "USDT");
                     if (price?.Price > 0)
                         tasks.Add(new(pair, price.Price));
                 }
             }
             return tasks;
         }

         public static async Task<List<(CryptoPair, double)>> UserTasksToNotify(UserConfig user, AppDbContext dbContext,
                 bool useInterval = true)
         {
             List<(CryptoPair, double)> tasks = new List<(CryptoPair, double)>();
             DateTime dateTimenow = DateTime.Now.ToUniversalTime().AddHours(user.TimezoneChange);

             if (!useInterval || (UpdateIntervalExpired(user.Id, user.NoticationsInterval)) &&
                 !NightTime(user.NightModeEnable, user.NightModeStartTime, user.NightModeEndsTime, dateTimenow))
             {
                 foreach (var pair in user.pairs.Where(x => x.OwnerId == user.Id && x.Enabled && !x.TriggerOnce)
                              .ToList())
                 {
                     var price = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());
                     if (price?.Price > 0 && (price.Price > pair.Price && pair.GainOrFall ||
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
                     if (price?.Price > 0 && (price.Price > pair.Price && pair.GainOrFall ||
                                              price.Price < pair.Price && !pair.GainOrFall))
                     {
                         pair.Triggered = true;
                         tasksReturing.Add(new(pair, price.Price));
                     }
                 }

                 await dbContext.SaveChangesAsync();
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

                 await dbContext.SaveChangesAsync();
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

         private static async Task CrtMsgMoon(List<(MonObj, double)> lst, UserConfig user)
         {
             StringBuilder sb = new StringBuilder();

             if (lst.Any())
             {
                 foreach (var pair in lst)
                     sb.AppendLine(FormatStrStock(pair.Item1, pair.Item2));

                 var userMsgH = lastUpdateUsers.FirstOrDefault(x => x.User.TelegramId == user.TelegramId);
                 if (userMsgH == null)
                 {
                     userMsgH = new UsersLastMessageIds(user);
                     lastUpdateUsers.Add(userMsgH);
                 }

                 if (user.RemoveLatestNotifyBeforeNew && userMsgH.MonitorMsg?.MsgId != null)
                 {
                     await BotApi.RemoveMessage(user.TelegramId, (int)userMsgH.MonitorMsg.MsgId);
                 }

                 var msg = await BotApi.SendMessage(user.TelegramId, sb.ToString());

                 if (msg != null)
                     userMsgH.MonitorMsg = new SentMsg(DateTime.Now.ToUniversalTime(), msg.MessageId);

                 //var lastMessage = new UsersLastMessageIds(user.Id, DateTime.Now.ToUniversalTime().AddHours(user.TimezoneChange), msg.MessageId);
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
             var lastUpdated = lastUpdateUsers.FirstOrDefault(x => x.User.Id == userId);
             if (lastUpdated?.NormalMsg == null) return true;

             if (lastUpdated.NormalMsg.SentDateTime + TimeSpan.FromSeconds(interval)
                 < DateTime.Now.ToUniversalTime())
             {
                 return true;
             }

             return false;
         }

         private static bool UpdateMonIntervalExpired(int userId, int interval)
         {
             var lastUpdated = lastUpdateUsers.FirstOrDefault(x => x.User.Id == userId);
             if (lastUpdated?.MonitorMsg == null) return true;

             var dateNow = DateTime.Now.ToUniversalTime();

             if (lastUpdated.MonitorMsg.SentDateTime + TimeSpan.FromSeconds(interval)
                 < dateNow)
             {
                 return true;
             }

             return false;
         }

         private static bool NightTime(bool nightEnable, TimeSpan start, TimeSpan end, DateTime now)
         {
             if (!nightEnable) return false;
             TimeSpan timeNow = now.TimeOfDay;
             CultureInfo provider = new CultureInfo("ru-RU");
             Thread.CurrentThread.CurrentCulture = provider;
             System.Globalization.DateTimeStyles style = DateTimeStyles.None;
             if (now.TimeOfDay.IsBetween(start, end))
             {
                 return true;
             }
             return false;
         }
     }

     internal static class TimeSpanExtensions
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

     public class UsersLastMessageIds
     {
         public UserConfig User { get; set; }
         public SentMsg NormalMsg { get; set; }
         public SentMsg MonitorMsg { get; set; }

         public UsersLastMessageIds(UserConfig user, long telegramId = 0)
         {
             User = user;
         }
     }

     public class SentMsg
     {
         public DateTime SentDateTime { get; set; }
         public int? MsgId { get; set; }

         public SentMsg(DateTime date, int? msgId)
         {
             MsgId = msgId;
             SentDateTime = date;
         }
     }*/
}