using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Telegram.Bot.Types;
using TelegramBot.Objects;

namespace TelegramBot.Static.MessageHandlers
{
    public class MonitorPairsMsgHandler : IMyDisposable
    {
        public async Task AddToMon(Update update, Match match)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pairBase = match.Groups["base"].Value;
                var newPair = new MonObj(pairBase.ToUpper());
                var userId = dbContext.Users.First(x => x.TelegramId == update.Message.From.Id);
                newPair.OwnerId = userId.Id;
                dbContext.MonPairs.Add(newPair);
                await dbContext.SaveChangesAsync();
                await BotApi.SendMessage(update.Message.Chat.Id, "Add pair to mon");
            }
        }

        public async Task RemoveFromMon(Update update, Match match)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pairBase = match.Groups["base"].Value.ToUpper();
                var userId = dbContext.Users.First(x => x.TelegramId == update.Message.From.Id);
                var newPair = dbContext.MonPairs.First(x => x.PairBase == pairBase && x.OwnerId == userId.Id);
                newPair.OwnerId = userId.Id;
                await dbContext.SaveChangesAsync();
                await BotApi.SendMessage(update.Message.Chat.Id, $"Removed {pairBase} pair from mon");
            }
        }
    }
}