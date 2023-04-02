using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using CryptoApi.Constants;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TelegramBot.Constants;
using TelegramBot.Objects;
using TelegramBot.Static.MessageHandlers;
using System.Text.RegularExpressions;
using Telegram.Bot.Polling;
using TelegramBot.Helpers;
using TelegramBot.Constants.Commands;
using TelegramBot.Services.MessageHandlers;

namespace TelegramBot.Static
{
    public class BotApi
    {
        public static bool IsBotStarted;
        public static TelegramBotClient BotClient { get; set; }
        public static CancellationTokenSource CancelToken { get; set; }

        /// <summary>
        /// Запустить бота.
        /// </summary>
        public static bool StartBot()
        {
            try
            {
                BotClient = new TelegramBotClient(AppSettingsStatic.Settings.TelegramBotToken);
                CancelToken = new CancellationTokenSource();

                var recievedOption = new ReceiverOptions()
                {
                    AllowedUpdates = { }
                };
                BotClient.StartReceiving(UpdateHandler, ErrorHandler, recievedOption, CancelToken.Token);
                IsBotStarted = true;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// TODO: Переделать.
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private static async Task SaveUserMsg(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                if (update.Type == UpdateType.Message)
                {
                    var userid = TelegramUpdatesHelper.GetTelegramIdFromUpdate(update);
                    var user = dbContext.Users.Include(x => x.Messages).OrderBy(x => x.Id).First(x => x.TelegramId == userid.Identifier);
                    if (update.Message?.Type != MessageType.Text) return;
                    var msg = new MessageAccepted(user, update.Message.Text, update.Message.MessageId);
                    msg.Date = update.Message.Date;
                    user.Messages?.Add(msg);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        #region Updates

        public static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken canceltoken)
        {
            //if (IsUserBanned(update))
            //    return;
            var user = await GetUserSettings(update);
            // await SaveUserMsg(update);

             if (!string.IsNullOrWhiteSpace(update.Message?.ReplyToMessage?.Text))
             {
                 using (RepliedMessagesHandler msgHandler = new RepliedMessagesHandler())
                 {
                     await msgHandler.HandleMessage(update);
                 }
             }
            else if (update.Type == UpdateType.CallbackQuery)
                CallbackHandlerAsync(bot, update, canceltoken);
            /* else if (update.Type == UpdateType.EditedMessage)
             {
                 update.Message = new Message();
                 update.Message.Text = update.EditedMessage.Text;
                 await MessageTextHandler(bot, update, canceltoken, user);
             }*/
            if (update.Type == UpdateType.Message && update.Message != null)
                await MessageTextHandler(bot, update, canceltoken, user);
        }

        public static async Task MessageTextHandler(ITelegramBotClient bot, Update update, CancellationToken token,
            UserConfig user)
        {
            // Обработка команды подписки. /subscribe
            if (update.Message?.Text == BreakoutCommands.Subscribe)
                using (BreakoutPairsMsgHandler msghandler = new BreakoutPairsMsgHandler())
                    await msghandler.SubNewUserBreakouts(update);

            // Обработка команды создания пары.
            else if (update.Message.Text == "/create")
                using (CryptoPairsMsgHandler msghandler = new CryptoPairsMsgHandler())
                    await msghandler.CreateTaskFirstStage(update, user);

            // Обработка команды включения ночного режима.
            else if (SettingsCommands.SetEnableNight.IsMatch(update.Message.Text))
                using (UserSettingsMsgHandler msgHandler = new())
                    await msgHandler.SetEnableNightMode(update);

            // Обработка команды включения
            else if (update.Message.Text == SimpleCommands.RemoveAllFromBlackList)
                using (BreakoutPairsMsgHandler brkMsgHandler = new BreakoutPairsMsgHandler())
                {
                    var result = await brkMsgHandler.RemoveAllBlackListedPairsUser(user);
                    await SendMessage(user.TelegramId, result.Message);
                }
            else if (update?.Message.Text == SimpleCommands.EnableCleaning)
                using (UserSettingsMsgHandler msgHandler = new())
                    await msgHandler.TurnLastMsgCleaning(update);
            else if (MonitoringTaskCommands.CreatePair.IsMatch(update.Message.Text))
                using (CryptoPairsMsgHandler msghandler = new CryptoPairsMsgHandler())
                    await msghandler.CreateTaskFirstStage(update, user);
            else if (BreakoutCommands.AddTopSymbolsToWhiteList.IsMatch(update.Message.Text))
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    await msgHandler.AddWhiteTopList(update);
            else if (update.Message.Text.Contains(SimpleCommands.FlipTasks))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.FlipTriggeredTasks(update);
            else if (update.Message.Text.Contains(SimpleCommands.ShiftTasks))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.DropEverythingByProcent(update);
            else if (update.Message?.Text == BreakoutCommands.SubSettings)
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    await msgHandler.SubSettings(update);
            else if (update.Message?.Text == BreakoutCommands.SubStop)
                using (BreakoutPairsMsgHandler msgH = new BreakoutPairsMsgHandler())
                    await msgH.StopNotify(update);
            else if (SimpleCommands.AllTasks == update.Message.Text)
                using (CryptoPairsMsgHandler cr = new CryptoPairsMsgHandler())
                    await cr.ListAllTask(update);
            else if (MonitoringTaskCommands.TriggerOncePair.IsMatch(update.Message.Text))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.SetSingleTriggerForUserTask(update);
            else if (MonitoringPairsCommands.AddMonPairsCommandRegex.IsMatch(update.Message.Text))
                using (MonitorPairsMsgHandler msghandler = new MonitorPairsMsgHandler())
                    await msghandler.AddToMon(update, MonitoringPairsCommands.AddMonPairsCommandRegex.Match(update.Message.Text));
            else if (MonitoringPairsCommands.DelMonPairsCommandRegex.IsMatch(update.Message.Text))
                using (MonitorPairsMsgHandler msghandler = new MonitorPairsMsgHandler())
                    await msghandler.RemoveFromMon(update, MonitoringPairsCommands.DelMonPairsCommandRegex.Match(update.Message.Text));
            else if (SettingsCommands.SetTimeZoneCommandRegex.IsMatch(update.Message.Text))
                using (UserSettingsMsgHandler msghandler = new UserSettingsMsgHandler())
                    await msghandler.SetTimeZone(update);
            else if (BreakoutCommands.AddTopSymbolsToWhiteList.IsMatch(update.Message.Text))
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    await msgHandler.AddWhiteTopList(update);
            else if (MonitoringTaskCommands.ShiftTasks.IsMatch(update.Message.Text))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.DropEverythingByProcent(update);
            else if (SettingsCommands.SetNightTime.IsMatch(update.Message.Text))
                using (UserSettingsMsgHandler msgHandler = new())
                    await msgHandler.SetNightTime(update);
            else if (MonitoringTaskCommands.ShowPair.IsMatch(update.Message.Text))
                using (CryptoPairsMsgHandler msgHanlder = new CryptoPairsMsgHandler())
                    await msgHanlder.ShowTaskInfo(update);
            else if (MonitoringTaskCommands.AddComment.IsMatch(update.Message.Text))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.AddCommentForTask(update);
            else if (SettingsCommands.ChangeDelay.IsMatch(update.Message.Text))
                using (SettingsManager sm = new SettingsManager())
                    await sm.SetNotifyDelay(update);
            else if (MonitoringTaskCommands.EditPair.IsMatch(update.Message.Text))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.EditUserTask(update);
            else if (MonitoringTaskCommands.DeletePair.IsMatch(update.Message.Text))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.RemoveUserTask(update);
            else if (BreakoutCommands.AddToBlackList.IsMatch(update.Message.Text))
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    await msgHandler.AddPairToBlackListCommandHandler(update);
            else if (BreakoutCommands.SetTimings.IsMatch(update.Message.Text))
                using (BreakoutPairsMsgHandler msg = new BreakoutPairsMsgHandler())
                    await msg.SetTimings(update);
        }

        public static async void CallbackHandlerAsync(ITelegramBotClient bot, Update update,
            CancellationToken cancellationToken)
        {
            await EditMessage(TelegramUpdatesHelper.GetTelegramIdFromUpdate(update), update.CallbackQuery.Message.MessageId, true);
            if (Exchanges.Contains(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    await msgh.SetExchangeCallbackHandlerStage(update);
            else if (CallbackDataPatterns.DeletePairRegex.IsMatch(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    await msgh.RemoveUserTaskCallbackHandler(update);
            else if (CallbackDataPatterns.EditPairRegex.IsMatch(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    await msgh.EditUserTaskCallbackHandler(update);
        }

        public static async Task ErrorHandler(ITelegramBotClient botClient, Exception ex, CancellationToken csToken)
        {
            ConsoleCommandsHandler.LogWrite(ex.Message);
            
        }

        public static async Task AddUserToBanList(ChatId chatid, string banReason)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                dbContext.BannedUsers.Add(new BannedUser()
                {
                    TelegramId = (long)chatid.Identifier,
                    UserName = chatid.Username,
                    BanReason = banReason
                });
                await dbContext.SaveChangesAsync();
                ConsoleCommandsHandler.LogWrite($"User {chatid.Identifier} banned with reason: {banReason}");
            }
        }

        #endregion Updates

        #region Send or edit

        /// <summary>
        /// Send message to user with parsing it with selected parse method
        /// </summary>
        /// <param name="chatId">User telegram id</param>
        /// <param name="message">Any text to user</param>
        /// <param name="parse">To select parse mode</param>
        /// <returns>Sent message</returns>
        public static async Task<Message> SendMessage(ChatId chatId, string message, ParseMode parse = ParseMode.MarkdownV2)
        {
            try
            {
                var messageLenght = message.Length;
                if (messageLenght > 4125)
                    for (int i = 0; i < messageLenght / 3500; i++)
                        return await BotClient.SendTextMessageAsync(chatId, message.Substring(i * 3500, i + 3500), parse,
                            disableWebPagePreview: true);
                else
                    return await BotClient.SendTextMessageAsync(chatId, message, parse, disableWebPagePreview: true);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                await BadRequestHandler(chatId, apiException);
            }
            catch (Telegram.Bot.Exceptions.RequestException reqException)
            {
                await BadRequestHandler(chatId, reqException);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Forbidden: bot was blocked by the user")
                {
                    await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
                }
            }
            return null;
        }

        /// <summary>
        /// Usually send long messages and automatically splits it in few messages.
        /// Not using any formating
        /// </summary>
        /// <param name="chatId">User telegram id</param>
        /// <param name="message">Any text to user</param>
        /// <returns>Message, which is send</returns>
        public static async Task<Message> SendMessage(ChatId chatId, string message)
        {
            try
            {
                var messagelenght = message.Length;
                if (messagelenght > 4125)
                    for (int i = 0; i < messagelenght / 3500; i++)
                        return await BotClient.SendTextMessageAsync(chatId, message.Substring(i * 3500, i + 3500));
                else
                    return await BotClient.SendTextMessageAsync(chatId, message);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                await BadRequestHandler(chatId, apiException);
            }
            catch (Telegram.Bot.Exceptions.RequestException reqException)
            {
                await BadRequestHandler(chatId, reqException);
            }
            return null;
        }

        /// <summary>
        /// Sending a message to the user and pinning the response to that message for the user to send back to the bot for further processing
        /// </summary>
        /// <param name="chatId">User telegram id</param>
        /// <param name="message">Any text to user</param>
        /// <param name="replyThis">Uses only to defining the path to this method</param>
        /// <returns>Sent message</returns>
        public static async Task<Message> SendMessage(ChatId chatId, string message, bool replyThis)
        {
            try
            {
                return await BotClient.SendTextMessageAsync(chatId, message, replyMarkup: new ForceReplyMarkup());
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                await BadRequestHandler(chatId, apiException);
            }
            catch (Telegram.Bot.Exceptions.RequestException reqException)
            {
                await BadRequestHandler(chatId, reqException);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Forbidden: bot was blocked by the user")
                {
                    await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
                }
            }
            return null;
        }

        /// <summary>
        /// Sending a message to the user and pinning the response to that message for the user to send back to the bot for further processing
        /// </summary>
        /// <param name="chatId">User telegram id</param>
        /// <param name="message">Any text to user</param>
        /// <param name="replyThis">Uses only to defining the path to this method</param>
        /// <param name="parse">To select parse mode</param>
        /// <returns>Sent message</returns>
        public static async Task<Message> SendMessage(ChatId chatId, string message, bool replyThis, ParseMode parse = ParseMode.MarkdownV2)
        {
            try
            {
                return await BotClient.SendTextMessageAsync(chatId, message, replyMarkup: new ForceReplyMarkup(), parseMode: parse);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                await BadRequestHandler(chatId, apiException);
            }
            catch (Telegram.Bot.Exceptions.RequestException reqException)
            {
                await BadRequestHandler(chatId, reqException);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Forbidden: bot was blocked by the user")
                {
                    await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
                }
            }
            return null;
        }

        /// <summary>
        /// Sending message to user with defined keyboard
        /// </summary>
        /// <param name="chatId">User telegram id</param>
        /// <param name="message">Any text to user</param>
        /// <param name="replyMarkup">Keyboard to attach it to the message</param>
        /// <returns></returns>
        public static async Task<Message> SendMessage(ChatId chatId, string message, IReplyMarkup replyMarkup)
        {
            try
            {
                return await BotClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                await BadRequestHandler(chatId, apiException);
            }
            catch (Telegram.Bot.Exceptions.RequestException reqException)
            {
                await BadRequestHandler(chatId, reqException);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Forbidden: bot was blocked by the user")
                {
                    await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
                }
            }
            return null;
        }

        /// <summary>
        /// Edits message by message id
        /// </summary>
        /// <param name="chatId">User telegram id</param>
        /// <param name="messageID">Any previous message id</param>
        /// <param name="newMessage">Any text to user</param>
        /// <returns>Sent message</returns>
        public static async Task<Message> EditMessage(ChatId chatId, int messageID, string newMessage)
        {
            try
            {
                return await BotClient.EditMessageTextAsync(chatId, messageID, newMessage);
            }
            catch (ApiRequestException apiEx)
            {
                await BadRequestHandler(chatId, apiEx);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Forbidden: bot was blocked by the user")
                {
                    await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
                }
            }
            return null;
        }

        /// <summary>
        /// Edits message by message id
        /// </summary>
        /// <param name="chatId">User telegram id</param>
        /// <param name="messageID">Any previous message id</param>
        /// <param name="newMessage">Any text to user</param>
        /// <param name="parseMode">To select parse mode</param>
        /// <returns>Sent message</returns>
        public static async Task<Message> EditMessage(ChatId chatId, int messageId, string message, ParseMode parseMode)
        {
            try
            {
                return await BotClient.EditMessageTextAsync(chatId, messageId, message, parseMode: parseMode, disableWebPagePreview: true);
            }
            catch (ApiRequestException apiEx)
            {
                await BadRequestHandler(chatId, apiEx);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Forbidden: bot was blocked by the user")
                {
                    await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
                }
            }
            return null;
        }

        /// <summary>
        /// Editing self message for revoking inline keyboard
        /// </summary>
        /// <param name="chatId">User chat id</param>
        /// <param name="messageID">Message id</param>
        /// <param name="removeKeyboard">New reply keyboard or null</param>
        /// <returns></returns>
        public static async Task EditMessage(ChatId chatId, int messageID, bool revokeKB)
        {
            try
            {
                await BotClient.EditMessageReplyMarkupAsync(chatId, messageID, null);
            }
            catch (ApiRequestException apiEx)
            {
                await BadRequestHandler(chatId, apiEx);
            }
        }

        /// <summary>
        /// Simply removes message by message id
        /// </summary>
        /// <param name="chatId">Which chat</param>
        /// <param name="msgId">Which message</param>
        /// <returns></returns>
        public static async Task RemoveMessage(ChatId chatId, int msgId)
        {
            try
            {
                await BotClient.DeleteMessageAsync(chatId, msgId);
            }
            catch (ApiRequestException apiEx)
            {
                await BadRequestHandler(chatId, apiEx);
            }
            catch (Telegram.Bot.Exceptions.RequestException) { }
            catch (Exception ex)
            {
                if (ex.Message == "Forbidden: bot was blocked by the user")
                {
                    await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
                }
            }
        }

        public static async Task BadRequestHandler(ChatId chatId, Exception ex)
        {
            if (ex.Message == "Bad Request: chat not found")
            {
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var baduser = dbContext.Users.FirstOrDefault(x => x.TelegramId == chatId.Identifier);
                    if (baduser != null) dbContext.Users.Remove(baduser);
                    ConsoleCommandsHandler.LogWrite($"Bad user {chatId.Identifier}. Removed from the database");
                }
            }
            else if (ex.Message == "Forbidden: bot was blocked by the user")
            {
                await AddUserToBanList(chatId, "Forbidden: bot was blocked by the user");
            }
            else if (ex.Message == "Exception during making request")
            {
                return;
            }
           
        }

        #endregion Send or edit

        #region UsersStuff

        public static bool IsUserBanned(Update update)
        {
            var userid = TelegramUpdatesHelper.GetTelegramIdFromUpdate(update);
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.BannedUsers?.ToList().FirstOrDefault(x => x.TelegramId == userid);
                if (user == null)
                    return false;
                //SendMessage(userid, "You are banned!");
                return true;
            }
        }

        public static async Task<UserConfig> GetUserSettings(Update update, AppDbContext dbContext = null)
        {
            var chatId = TelegramUpdatesHelper.GetTelegramIdFromUpdate(update);
            if (dbContext == null) dbContext = new AppDbContext();

            var user = dbContext.Users.FirstOrDefault(x => x.TelegramId == chatId.Identifier);
            if (user == null)
            {
                user = new UserConfig()
                {
                    TelegramId = (long)chatId.Identifier,
                    UserName = update.Message?.From?.Username ?? "",
                    FirstName = update.Message?.From?.FirstName + " " + update.Message?.From?.LastName
                };
                dbContext.Users.Add(user);
                await SendMessage(chatId, Messages.welcomeMsg);
                await dbContext.SaveChangesAsync();
            }

            return user;
        }

        #endregion UsersStuff
    }
}