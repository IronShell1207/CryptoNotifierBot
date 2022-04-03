using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBot.Static
{
    public class BotApi
    {
        public static bool IsBotStarted;
        public static TelegramBotClient BotClient { get; set; }
        public static CancellationTokenSource CancelToken { get; set; }

        public static async Task<bool> StartBotAsync(string token)
        {
            try
            {
                BotClient = new TelegramBotClient(token);
                CancelToken = new CancellationTokenSource();
                var DateNow = DateTime.Now.ToLongTimeString();

                var RecievedOption = new ReceiverOptions()
                {
                    AllowedUpdates = { }
                };
                BotClient.StartReceiving(UpdateHandler, ErrorHandler, RecievedOption, CancelToken.Token);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken canceltoken)
        {

        }
        public static Task ErrorHandler(ITelegramBotClient botClient, Exception ex, CancellationToken csToken)
        {
            Console.WriteLine(ex.Message);
            return Task.CompletedTask;
        }
        public static async Task SendMessage(ChatId chatId, string message)
        {
            await BotClient.SendTextMessageAsync(chatId, message);
        }

        public static async Task SendMessage(ChatId chatId, string message, IReplyMarkup replyMarkup)
        {
            await BotClient.SendTextMessageAsync(chatId, message,replyMarkup: replyMarkup);
        }

    }
}
