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
                var user = BotApi.GetUserSettings(BotApi.GetTelegramIdFromUpdate(update)).Result;
                if (user != null)
                {
                    user.NoticationsInterval = int.Parse(match.Groups["time"].Value);
                    BotApi.SendMessage(user.TelegramId, MessagesGetter.GetGlobalString("SetNotifyInterval"))

                }
            }
        }
    }
}
