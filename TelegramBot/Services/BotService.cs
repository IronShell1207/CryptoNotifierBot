using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Helpers;

namespace TelegramBot.Services
{
    public class BotService
    {
        /// <summary>
        /// Запущен ли бот.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Статичный экземпляр класса <see cref="BotService"/>.
        /// </summary>
        private static readonly Lazy<BotService> _instance = new((() => new BotService()));

        /// <summary>
        /// Статичный экземпляр класса <see cref="BotService"/>.
        /// </summary>
        public static BotService Instance => _instance.Value;

        /// <summary>
        /// токен отмены бота
        /// </summary>
        public CancellationTokenSource BotCancelToken => new CancellationTokenSource();

        public async Task<bool> StartBotServer()
        {
            var botClient = new TelegramBotClient(AppSettingsHelper.Instance.Settings.TelegramBotToken);
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            botClient.StartReceiving(HandleUpdatesAsync, HandlePollingErrorAsync, receiverOptions, BotCancelToken.Token);

            var me = await botClient.GetMeAsync();
            return true;
        }

        private async Task HandleUpdatesAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
        }

        private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
        }
    }
}