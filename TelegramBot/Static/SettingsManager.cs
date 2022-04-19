using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Objects;
using Telegram.Bot.Types;
using TelegramBot.Constants;

namespace TelegramBot.Static
{
    public class SettingsManager : IMyDisposable
    {
        public async void SetNotifyDelay(Update update)
        {
            var match = CommandsRegex.SettingsCommands.ChangeDelay.Match(update.Message.Text);
            if (match.Success)
            {
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var userId = BotApi.GetTelegramIdFromUpdate(update);
                    var user = dbContext.Users.FirstOrDefault(x=>x.TelegramId == userId.Identifier);
                    if (user != null)
                    {
                        user.NoticationsInterval = int.Parse(match.Groups["time"].Value);
                        dbContext.SaveChangesAsync();
                        BotApi.SendMessage(user.TelegramId, string.Format(MessagesGetter.GetSettingsMsgString("SetNotifyInterval", user.Language),
                            user.NoticationsInterval));
                    }
                }
            }
        }
    }
}
