using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Objects;
using Telegram.Bot.Types;
using TelegramBot.Constants.Commands;
using TelegramBot.Helpers;

namespace TelegramBot.Static
{
    public class SettingsManager : IMyDisposable
    {
        /*public async Task SetNotifyDelay(Update update)
        {
            var match = SettingsCommands.ChangeDelay.Match(update.Message.Text);
            if (match.Success)
            {
                using (var dbContext = new AppDbContext())
                {
                    var userId = TelegramUpdatesHelper.GetTelegramIdFromUpdate(update);
                    var user = dbContext.Users.FirstOrDefault(x => x.TelegramId == userId.Identifier);
                    if (user != null)
                    {
                        user.NoticationsInterval = int.Parse(match.Groups["time"].Value);
                        await dbContext.SaveChangesAsync();
                        await BotApi.SendMessage(user.TelegramId, string.Format(CultureTextRequest.GetSettingsMsgString("SetNotifyInterval", user.Language),
                            user.NoticationsInterval));
                    }
                }
            }
        }*/
    }
}