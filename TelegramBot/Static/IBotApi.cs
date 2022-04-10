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
using CryptoApi.Constants;
using Telegram.Bot;
using TelegramBot.Constants;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class BotApi
    {
        public static bool IsBotStarted;
        public static TelegramBotClient BotClient { get; set; }
        public static CancellationTokenSource CancelToken { get; set; }
        public static async Task<bool> StartBotAsync()
        {
            try
            {
                BotClient = new TelegramBotClient(AppSettingsStatic.Settings.TelegramBotToken);
                CancelToken = new CancellationTokenSource();
                var DateNow = DateTime.Now.ToLongTimeString();

                var RecievedOption = new ReceiverOptions()
                {
                    AllowedUpdates = { }
                };
                BotClient.StartReceiving(UpdateHandler, ErrorHandler, RecievedOption, CancelToken.Token);
                IsBotStarted = true;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken canceltoken)
        {
            if (update.Type == UpdateType.Message)
            {
                var user = GetUserSettings(update.Message.Chat.Id);
                if (CommandsRegex.CreatePair.IsMatch(update.Message.Text))
                {
                    using (BotMessageHandler msghandler = new BotMessageHandler())
                    {
                        msghandler.AddCryptoPair(update);
                    }
                }
               // else if (update.Message.ReplyToMessage.Text == "")
              //  {
                    
              //  }
                else if (update.Message.Text == "/subscribe")
                {
                    using (BotMessageHandler msghandler = new BotMessageHandler())
                    {
                        msghandler.SubNewUserBreakouts(update);
                    }
                }
                else if (update.Message.Text == "/subsets")
                {
                    using (BotMessageHandler msgHandler = new BotMessageHandler())
                    {
                        msgHandler.SubSettings(update);
                    }
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                if (Exchanges.Contains(update.CallbackQuery.Data))
                {
                    using (BotMessageHandler msgHandler = new BotMessageHandler())
                    {
                        msgHandler.SelectingCryptoExchangeForPairQueryHandler(update);
                    }
                }
            }
        }
        public static Task ErrorHandler(ITelegramBotClient botClient, Exception ex, CancellationToken csToken)
        {
            Console.WriteLine(ex.Message);
            return Task.CompletedTask;
        }
        public static async Task SendMessage(ChatId chatId, string message)
        {
            try
            {
                var messagelenght = message.Length;
                if (messagelenght > 4125)
                    for (int i =0; i< messagelenght; i+=4000)
                        await BotClient.SendTextMessageAsync(chatId, message.Substring(i, i + 4000));
                else
                    await BotClient.SendTextMessageAsync(chatId, message);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                if (apiException.Message == "Bad Request: chat not found" || apiException.ErrorCode == 400)
                {
                    using (AppDbContext dbContext = new AppDbContext())
                    {
                        var baduser =  dbContext.Users.ToList().Where(x => x.TelegramId == chatId.Identifier)
                            .FirstOrDefault();
                        if (baduser != null) dbContext.Users.Remove(baduser);
                        Console.WriteLine($"Bad user {chatId.Identifier}. Removed from the database");
                    }
                }
            }
        }

        public static async Task SendMessage(ChatId chatId, string message, bool replythis)
        {
            var msg = await BotClient.SendTextMessageAsync(chatId, message, replyMarkup: new ForceReplyMarkup());
        }

        public static async Task SendMessage(ChatId chatId, string message, IReplyMarkup replyMarkup)
        {
            await BotClient.SendTextMessageAsync(chatId, message,replyMarkup: replyMarkup);
        }

        public static async Task<UserConfig> GetUserSettings(ChatId chatId)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.Users.ToList().FirstOrDefault(x => x.TelegramId == chatId.Identifier);
                if (user == null)
                {
                    user = new UserConfig()
                    {
                        TelegramId = (long)chatId.Identifier
                    };
                    db.Users.Add(user);
                    await SendMessage(chatId, Messages.welcomeMsg);
                    await db.SaveChangesAsync();
                }
                return user;
            }
        }

    }
}
