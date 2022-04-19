using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
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
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TelegramBot.Constants;
using TelegramBot.Objects;
using TelegramBot.Static.MessageHandlers;

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
            if (!string.IsNullOrWhiteSpace(update.Message?.ReplyToMessage?.Text))
            {
                RepliedMsgHandlerAsync(bot, update, canceltoken);
            }
            if (update.Type == UpdateType.Message && update.Message != null)
            {
                var user = GetUserSettings(update.Message?.Chat?.Id);
                if (RegexCombins.CommandPattern.IsMatch(update.Message?.Text))
                    CommandsHandler(bot, update, canceltoken);
                else if (CommandsRegex.MonitoringTaskCommands.CreatePair.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msghandler = new CryptoPairsMsgHandler()) msghandler.NewCP(update);
                else if (CommandsRegex.SettingsCommands.ChangeDelay.IsMatch(update.Message.Text))   using (SettingsManager sm = new SettingsManager()) sm.SetNotifyDelay(update);
                else if (CommandsRegex.MonitoringTaskCommands.EditPair.IsMatch(update.Message.Text)) {}
                   
                else if (CommandsRegex.MonitoringTaskCommands.DeletePair.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler()) msgHandler.RemoveUserTask(update);
               
                else if (CommandsRegex.BreakoutCommands.AddToBlackList.IsMatch(update.Message.Text))
                {
                    using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    {
                        msgHandler.AddPairToBlackListCommandHandler(update);
                    }
                }
                else if (CommandsRegex.SetTimings.IsMatch(update.Message.Text))
                {
                    using (BreakoutPairsMsgHandler msg = new BreakoutPairsMsgHandler())
                    {
                        msg.SetTimings(update);
                    }
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
                CallbackHandlerAsync(bot,update, canceltoken);
        }

        public static async void CommandsHandler(ITelegramBotClient bot, Update update,
            CancellationToken cancellationToken)
        {
            if (update.Message?.Text == Commands.Subscribe)
                using (BreakoutPairsMsgHandler msghandler = new BreakoutPairsMsgHandler()) msghandler.SubNewUserBreakouts(update);
            else if (update.Message?.Text == Commands.SubSettings)
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler()) msgHandler.SubSettings(update);
            else if (update.Message?.Text == Commands.SubStop)
                using (BreakoutPairsMsgHandler msgH = new BreakoutPairsMsgHandler()) msgH.StopNotify(update);
            else if (Commands.AllTasks == update.Message.Text)
                using (CryptoPairsMsgHandler cr = new CryptoPairsMsgHandler()) cr.ListAllTask(update);
        }
        public static async void RepliedMsgHandlerAsync(ITelegramBotClient bot, Update update,
            CancellationToken canceltoken)
        {
            var user = GetUserSettings(update.Message.Chat.Id).Result;
            if (update.Message.ReplyToMessage.Text == Messages.newPairRequestingForPair)
            {
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                {
                    msgh.CreatingPairStageCP(update);
                }
            }
            else if (update.Message.ReplyToMessage.Text == Messages.newPairWrongPrice)
            {
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                {
                    msgh.SetTriggerPriceStageCP(update);
                }
            }
            else if (update.Message.ReplyToMessage.Text == Messages.newPairAfterExchangeSetPrice)
            {
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                {
                    msgh.SetTriggerPriceStageCP(update);
                }
            }
            else if (update.Message.ReplyToMessage.Text ==
                     CultureTextRequest.GetMessageString("ToaddToTheBlackList", user.Language))
            {
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                {
                    msgHandler.AddPairToBlackListCommandHandler(update);
                }
            }
        }
        public static async void CallbackHandlerAsync(ITelegramBotClient bot, Update update,
            CancellationToken cancellationToken)
        {
            EditMessage(GetTelegramIdFromUpdate(update), update.CallbackQuery.Message.MessageId, true);
            if (Exchanges.Contains(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler()) msgh.SetExchangeStageCP(update);
            else if (CallbackDataPatterns.DeletePairRegex.IsMatch(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler()) msgh.RemoveUserTaskCallbackHandler(update);
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
                    for (int i =0; i< messagelenght/3500; i++)
                        await BotClient.SendTextMessageAsync(chatId, message.Substring(i*3500, i + 3500));
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
        public static ChatId GetTelegramIdFromUpdate(Update update)
        {
            if (update.Message?.Chat.Id != null)
                return update.Message.Chat.Id;
            else if (update.CallbackQuery?.From?.Id != null)
                return update.CallbackQuery.From.Id;
            else return null;
        }
        public static async Task SendMessage(ChatId chatId, string message, bool replythis)
        {
            try
            {
                var msg = await BotClient.SendTextMessageAsync(chatId, message, replyMarkup: new ForceReplyMarkup());
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                if (apiException.Message == "Bad Request: chat not found" || apiException.ErrorCode == 400)
                {
                    using (AppDbContext dbContext = new AppDbContext())
                    {
                        var baduser = dbContext.Users.FirstOrDefault(x => x.TelegramId == chatId.Identifier);
                        if (baduser != null) dbContext.Users.Remove(baduser);
                        Console.WriteLine($"Bad user {chatId.Identifier}. Removed from the database");
                    }
                }
            }
        }

        public static async Task SendMessage(ChatId chatId, string message, IReplyMarkup replyMarkup)
        {
            try
            {
                await BotClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                if (apiException.Message == "Bad Request: chat not found" || apiException.ErrorCode == 400)
                {
                    using (AppDbContext dbContext = new AppDbContext())
                    {
                        var baduser = dbContext.Users.FirstOrDefault(x => x.TelegramId == chatId.Identifier);
                        if (baduser != null) dbContext.Users.Remove(baduser);
                        Console.WriteLine($"Bad user {chatId.Identifier}. Removed from the database");
                    }
                }
            }
        }

        public static async Task EditMessage(ChatId chatId, string newMessage, int messageID)
        {
            await BotClient.EditMessageTextAsync(chatId, messageID, newMessage);
            //try
            //{

            //}
            //catch (Exception ex) 
            //{

            //}
        }
        /// <summary>
        /// Editing self message for revoking inline keyboard
        /// </summary>
        /// <param name="chatId">User chat id</param>
        /// <param name="messageID">Message id</param>
        /// <param name="removeKeyboard">New reply keyboard or null</param>
        /// <returns></returns>
        public static async Task EditMessage(ChatId chatId, int messageID, bool removeKeyboard)
        {
            await BotClient.EditMessageReplyMarkupAsync(chatId, messageID, null);
        }

        public static async Task<UserConfig> GetUserSettings(int userId)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    return null;
                }
                return user;
            }
        }
        public static async Task<UserConfig> GetUserSettings(ChatId chatId)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(x => x.TelegramId == chatId.Identifier);
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
