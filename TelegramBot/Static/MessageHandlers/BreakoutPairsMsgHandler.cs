using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Objects;

namespace TelegramBot.Static.MessageHandlers
{
    public class BreakoutPairsMsgHandler : IMyDisposable
    {
        public void StopNotify(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.BreakoutSubs.ToList().FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                if (user.Subscribed)
                {
                    user.Subscribed = false;
                    db.SaveChangesAsync();
                    BotApi.SendMessage(user.TelegramId, "Your breakouts subscription deactivated. To start subscription send /subscribe");
                }
            }
        }
        public async void SubNewUserBreakouts(Update update)
        {
            var user = new BreakoutSub()
            {
                TelegramId = update.Message.Chat.Id
            };
            using (AppDbContext db = new AppDbContext())
            {
                var sub = db.BreakoutSubs.ToList().FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                if (sub == null)
                {
                    db.BreakoutSubs.Add(user);
                    db.SaveChangesAsync();
                    await BotApi.SendMessage(update.Message.Chat.Id, string.Format(Messages.subscribedSucs,
                        $"\nGate IO platform: {user.GateioSub}\nBinance platform: {user.BinanceSub}\nOkx platfrom: {user.OkxSub}\nKucoin platform: {user.KucoinSub}"));
                }
                else if (sub.Subscribed)
                {
                    
                    await BotApi.SendMessage(update.Message.Chat.Id, "You already subscribed to breakout bot. Your current settings: " +
                                                                     $"\nGate IO platform: {user.GateioSub}\nBinance platform: {user.BinanceSub}\nOkx platfrom: {user.OkxSub}\nKucoin platform: {user.KucoinSub}");
                }
                else if (!sub.Subscribed)
                {
                    sub.Subscribed = true;
                    db.SaveChangesAsync();
                    await BotApi.SendMessage(update.Message.Chat.Id, string.Format(Messages.subscribedSucs,
                        $"\nGate IO platform: {user.GateioSub}\nBinance platform: {user.BinanceSub}\nOkx platfrom: {user.OkxSub}\nKucoin platform: {user.KucoinSub}"));
                }

            }
        }

        public async void SetTimings(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var sub = db.BreakoutSubs.FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                var match = CommandsRegex.SetTimings.Match(update.Message.Text);
                var timing = double.Parse( match.Groups["timing"].Value);
                switch (timing)
                {
                    case 2:
                        sub.S2MinUpdates = !sub.S2MinUpdates;
                        break;
                    case 5:
                        sub.S5MinUpdates = !sub.S5MinUpdates;
                        break;
                    case 15:
                        sub.S15MinUpdates = !sub.S15MinUpdates;
                        break;
                    case 30:
                        sub.S30MinUpdates = !sub.S30MinUpdates;
                        break;
                    case 45:
                        sub.S45MinUpdates = !sub.S45MinUpdates;
                        break;
                    case 60:
                        sub.S60MinUpdates = !sub.S60MinUpdates;
                        break;
                }
            }
        }
        public async void SubSettings(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var subber = db.BreakoutSubs.ToList().Find(x => x.TelegramId == update.Message.Chat.Id);
                if (subber != null)
                {
                    var message = $"{21}";
                    BotApi.SendMessage(update.Message.Chat.Id, message, true);
                }
            }
        }

    }
}
