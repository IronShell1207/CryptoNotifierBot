using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Objects;
using CryptoPair = TelegramBot.Objects.CryptoPair;

namespace TelegramBot.Static
{
    public class BotMessageHandler : IMyDisposable
    {

        public async void SettingPriceForNewTask(Update update)
        {
            var user = await BotApi.GetUserSettings(update.Message.Chat.Id);

        }

    }
}
