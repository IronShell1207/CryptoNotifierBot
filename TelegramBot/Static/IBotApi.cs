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

        private static void SaveUserMsg(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var user = dbContext.Users.Include(x=>x.Messages).OrderBy(x => x.Id).First(x => x.TelegramId == update.Message.From.Id);
                if (update.Message?.Type != MessageType.Text ) return;
                var msg = new MessageAccepted(user, update.Message.Text, update.Message.MessageId);
                msg.Date = update.Message.Date;
                user.Messages.Add(msg);
                dbContext.SaveChangesAsync();
            }
        }
        #region Updates
        public static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken canceltoken)
        {
            var user = await GetUserSettings(update);
            SaveUserMsg(update);
            if (IsUserBanned(update))
                return;
            if (!string.IsNullOrWhiteSpace(update.Message?.ReplyToMessage?.Text))
            {
                RepliedMsgHandlerAsync(bot, update, canceltoken);
            }

            if (update.Type == UpdateType.Message && update.Message != null)
            {
               
                if (RegexCombins.CommandPattern.IsMatch(update.Message?.Text))
                    CommandsHandler(bot, update, canceltoken);

                else if (CommandsRegex.MonitoringTaskCommands.ShiftTasks.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                        msgHandler.DropEverythingByProcent(update);

                else if (CommandsRegex.MonitoringTaskCommands.ShowPair.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msgHanlder = new CryptoPairsMsgHandler())
                        msgHanlder.ShowTaskInfo(update);

                else if (CommandsRegex.MonitoringTaskCommands.AddComment.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                        msgHandler.AddCommentForTask(update);

                else if (CommandsRegex.MonitoringTaskCommands.CreatePair.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msghandler = new CryptoPairsMsgHandler())
                        msghandler.NewCP(update);

                else if (CommandsRegex.SettingsCommands.ChangeDelay.IsMatch(update.Message.Text))
                    using (SettingsManager sm = new SettingsManager())
                        sm.SetNotifyDelay(update);

                else if (CommandsRegex.MonitoringTaskCommands.EditPair.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                        msgHandler.EditUserTask(update);

                else if (CommandsRegex.MonitoringTaskCommands.DeletePair.IsMatch(update.Message.Text))
                    using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                        msgHandler.RemoveUserTask(update);

                else if (CommandsRegex.BreakoutCommands.AddToBlackList.IsMatch(update.Message.Text))
                    using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                        msgHandler.AddPairToBlackListCommandHandler(update);

                else if (CommandsRegex.SetTimings.IsMatch(update.Message.Text))
                    using (BreakoutPairsMsgHandler msg = new BreakoutPairsMsgHandler())
                        msg.SetTimings(update);

            }
            else if (update.Type == UpdateType.CallbackQuery)
                CallbackHandlerAsync(bot, update, canceltoken);
        }

        public static async void CommandsHandler(ITelegramBotClient bot, Update update,
            CancellationToken cancellationToken)
        {
            if (update.Message?.Text == Commands.Subscribe)
                using (BreakoutPairsMsgHandler msghandler = new BreakoutPairsMsgHandler())
                    msghandler.SubNewUserBreakouts(update);

            else if (update.Message.Text.Contains(Commands.FlipTasks))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    msgHandler.FlipTriggeredTasks(update);

            else if (update.Message.Text.Contains(Commands.ShiftTasks))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    msgHandler.DropEverythingByProcent(update);

            else if (update.Message?.Text == Commands.SubSettings)
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    msgHandler.SubSettings(update);

            else if (update.Message?.Text == Commands.SubStop)
                using (BreakoutPairsMsgHandler msgH = new BreakoutPairsMsgHandler())
                    msgH.StopNotify(update);

            else if (Commands.AllTasks == update.Message.Text)
                using (CryptoPairsMsgHandler cr = new CryptoPairsMsgHandler())
                    cr.ListAllTask(update);
        }

        public static async void RepliedMsgHandlerAsync(ITelegramBotClient bot, Update update,
            CancellationToken canceltoken)
        {
            var user = GetUserSettings(update).Result;
            var editPriceMsgRegex =
                CommandsRegex.ConvertMessageToRegex(CultureTextRequest.GetMessageString("CPEditPair", user.Language),
                    new List<string>()
                        {@"(?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})", "(?<id>[0-9]+)"});

            if (update.Message.ReplyToMessage.Text == Messages.newPairRequestingForPair)
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    msgh.CreatingPairStageCP(update);

            else if (update.Message.ReplyToMessage.Text == Messages.newPairWrongPrice)
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    msgh.SetTriggerPriceStageCP(update);

            else if (update.Message.ReplyToMessage.Text == Messages.newPairAfterExchangeSetPrice)
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    msgh.SetTriggerPriceStageCP(update);

            else if (update.Message.ReplyToMessage.Text ==
                     CultureTextRequest.GetMessageString("ToaddToTheBlackList", user.Language))
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    msgHandler.AddPairToBlackListCommandHandler(update);

            else if (editPriceMsgRegex.IsMatch(update.Message.ReplyToMessage.Text))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    msgHandler.EditUserTaskReplyHandler(update, user);
        }

        public static async void CallbackHandlerAsync(ITelegramBotClient bot, Update update,
            CancellationToken cancellationToken)
        {
            EditMessage(GetTelegramIdFromUpdate(update), update.CallbackQuery.Message.MessageId, true);
            if (Exchanges.Contains(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    msgh.SetExchangeStageCP(update);

            else if (CallbackDataPatterns.DeletePairRegex.IsMatch(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    msgh.RemoveUserTaskCallbackHandler(update);

            else if (CallbackDataPatterns.EditPairRegex.IsMatch(update.CallbackQuery.Data))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    msgh.EditUserTaskCallbackHandler(update);
        }

        public static Task ErrorHandler(ITelegramBotClient botClient, Exception ex, CancellationToken csToken)
        {
            ConsoleCommandsHandler.LogWrite(ex.Message);
            return Task.CompletedTask;
        }

        #endregion

        #region Send or edit

        public static async Task<Message> SendMessage(ChatId chatId, string message, ParseMode parse = ParseMode.MarkdownV2)
        {
            try
            {
                var messagelenght = message.Length;
                if (messagelenght > 4125)
                    for (int i = 0; i < messagelenght / 3500; i++)
                        return await BotClient.SendTextMessageAsync(chatId, message.Substring(i * 3500, i + 3500), parse,
                            disableWebPagePreview: true);
                else
                    return await BotClient.SendTextMessageAsync(chatId, message, parse, disableWebPagePreview: true);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                await BadRequestHandler(chatId, apiException);
            }

            return null;
        }

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
            return null;
        }

      
        public static async Task<Message> SendMessage(ChatId chatId, string message, bool replythis)
        {
            try
            {
                return await BotClient.SendTextMessageAsync(chatId, message, replyMarkup: new ForceReplyMarkup());
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException apiException)
            {
                await BadRequestHandler(chatId, apiException);
            }
            return null;

        }

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
            return null;
        }

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
            return null;
        }

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
        }
        public static ChatId GetTelegramIdFromUpdate(Update update)
        {
            if (update.Message?.Chat.Id != null)
                return update.Message.Chat.Id;
            else if (update.CallbackQuery?.From?.Id != null)
                return update.CallbackQuery.From.Id;
            else return null;
        }


        public static async Task BadRequestHandler(ChatId chatId, Telegram.Bot.Exceptions.ApiRequestException ex)
        {
            if (ex.Message == "Bad Request: chat not found" || ex.ErrorCode == 400)
            {
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var baduser = dbContext.Users.FirstOrDefault(x => x.TelegramId == chatId.Identifier);
                    if (baduser != null) dbContext.Users.Remove(baduser);
                    ConsoleCommandsHandler.LogWrite($"Bad user {chatId.Identifier}. Removed from the database");
                }
            }
            else throw ex;
        }

        #endregion

        #region UsersStuff

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

        public static bool IsUserBanned(Update update)
        {
            var userid = GetTelegramIdFromUpdate(update);
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.BannedUsers?.ToList().FirstOrDefault(x => x.TelegramId == userid);
                if (user == null)
                    return false;
                SendMessage(userid, "You are banned!");
                return true;
            }
        }
        public static async Task<UserConfig> GetUserSettings(Update update)
        {
            var chatId = GetTelegramIdFromUpdate(update);
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(x => x.TelegramId == chatId.Identifier);
                if (user == null)
                {
                    user = new UserConfig()
                    {
                        TelegramId = (long)chatId.Identifier,
                        UserName = update.Message?.From?.Username ?? "",
                        FirstName = update.Message?.From?.FirstName + " " + update.Message?.From?.LastName
                    };
                    db.Users.Add(user);
                    await SendMessage(chatId, Messages.welcomeMsg);
                    db.SaveChangesAsync();
                }

                return user;
            }
        }

        public static async Task<int> GetMessageIdFromUpdateTask(Update update)
        {
            if (update.Message?.MessageId != null) return update.Message.MessageId;
            else if (update.CallbackQuery?.Message?.MessageId != null) return update.CallbackQuery.Message.MessageId;
            else return 0;
        }

        #endregion
    }
}
