using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Constants;
using TelegramBot.Objects;
using TelegramBot.Static.BotLoops;
using TelegramBot.Static.DbOperations;

namespace TelegramBot.Static.MessageHandlers
{
    public class CryptoPairsMsgHandler : IMyDisposable
    {
        private async Task<CryptoPair> GetTempUserTask(Update update)
        {
            var user = await BotApi.GetUserSettings(update);
            CryptoPair pair = PairsManager.TempObjects?.FirstOrDefault(x => x.OwnerId == user.Id);
            if (pair == null)
            {
                pair = new CryptoPair(user.Id);
                PairsManager.TempObjects.Add(pair);
            }
            return pair;
        }

        #region TasksEditChange
        public async void EditUserTask(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.EditPair.Match(update.Message.Text);
            var user = BotApi.GetUserSettings(update).Result;
            if (match.Success)
            {
                var id = match.Groups["id"].Value;
                var price = match.Groups["price"].Value;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    int iid = int.Parse(id);
                    using (CryptoPairDbHandler dbh = new CryptoPairDbHandler())
                    {
                        var task = dbh.GetPairFromId(iid, user.Id);
                        if (task != null)
                        {
                            if (!string.IsNullOrWhiteSpace(price))
                            {
                                if (await dbh.SetNewPriceTriggerPair(iid, user.Id, double.Parse(price)))
                                {
                                    var msg = string.Format(
                                        CultureTextRequest.GetMessageString("CPEditTaskComplete", user.Language),
                                        task.FullTaskInfo());
                                    BotApi.SendMessage(user.TelegramId, msg, ParseMode.Html);
                                }
                                else
                                {
                                    BotApi.SendMessage(user.TelegramId, "Error");
                                }
                            }
                            else
                            {
                                var msg = string.Format(
                                    CultureTextRequest.GetMessageString("CPEditPair", user.Language), task.TaskStatus(), task.Id);
                                BotApi.SendMessage(user.TelegramId, msg, true);
                            }
                        }
                    }
                }
                else
                {
                    var pairbase = match.Groups["base"].Value.ToUpper();
                    var pairquote = match.Groups["quote"].Value.ToUpper();
                    if (!string.IsNullOrWhiteSpace(pairbase) && !string.IsNullOrWhiteSpace(pairquote))
                    {
                        TradingPair pair = new TradingPair(pairbase, pairquote);
                        var strMessage = CultureTextRequest.GetMessageString("CPEditbySymbol", user.Language);
                        ListMatchingTasks(pair, user, CallbackDataPatterns.EditPair, strMessage);
                    }
                    else if (!string.IsNullOrWhiteSpace(pairbase))
                    {
                        var strMessage = CultureTextRequest.GetMessageString("CPEditbySymbol", user.Language);
                        ListMatchingTasks(pairbase, user, CallbackDataPatterns.EditPair, strMessage);
                    }
                    else
                    {
                        BotApi.SendMessage(user.TelegramId, CultureTextRequest.GetMessageString("CPEditEmpty", user.Language));
                    }
                }
            }
        }
        public async void EditUserTaskReplyHandler(Update update, UserConfig user)
        {
            var editPriceMsgRegex =
                CommandsRegex.ConvertMessageToRegex(CultureTextRequest.GetMessageString("CPEditPair", user.Language),
                    new List<string>() { @"(?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})", "(?<id>[0-9]+)" });
            var match = editPriceMsgRegex.Match(update.Message.ReplyToMessage.Text);
            if (match.Success)
            {
                var id = int.Parse(match.Groups["id"].Value);
                var priceStr = update.Message.Text;
                double price;
                if (double.TryParse(priceStr, out price))
                {
                    using (CryptoPairDbHandler dbHandler = new CryptoPairDbHandler())
                    {
                        var pair = dbHandler.GetPairFromId(id, user.Id);
                        if (pair != null)
                        {
                            var e = await dbHandler.SetNewPriceFromPair(pair, price);
                            var msg = string.Format(
                                CultureTextRequest.GetMessageString("CPEditTaskComplete", user.Language),
                                pair.FullTaskInfo());
                            BotApi.SendMessage(user.TelegramId, msg, ParseMode.Html);
                        }
                    }
                }
            }
            else BotApi.SendMessage(user.TelegramId, "Pair doesnt exists");

        }
        public void EditUserTaskCallbackHandler(Update update)
        {
            var match = CallbackDataPatterns.EditPairRegex.Match(update.CallbackQuery.Data);
            var user = BotApi.GetUserSettings(update).Result;
            if (match.Success)
            {
                var Id = int.Parse(match.Groups["id"].Value);
                var userId = int.Parse(match.Groups["ownerId"].Value);

                if (userId == user.Id)
                {
                    using (CryptoPairDbHandler dbHandler = new CryptoPairDbHandler())
                    {
                        var pair = dbHandler.GetPairFromId(Id, userId);
                        if (pair != null)
                        {
                            var msg = string.Format(CultureTextRequest.GetMessageString("CPEditPair", user.Language),
                                pair.ToString(), pair.Id);
                            BotApi.SendMessage(user.TelegramId, msg, true);
                        }

                    }
                }
            }
            else BotApi.SendMessage(user.TelegramId, "Pair doesnt exists");
        }
        public async void RemoveUserTaskCallbackHandler(Update update)
        {
            var match = CallbackDataPatterns.DeletePairRegex.Match(update.CallbackQuery.Data);
            var user = BotApi.GetUserSettings(update).Result;
            if (match.Success)
            {
                var Id = int.Parse(match.Groups["id"].Value);
                var userId = int.Parse(match.Groups["ownerId"].Value);
                if (userId == user.Id)
                {
                    using (CryptoPairDbHandler dbh = new CryptoPairDbHandler())
                    {
                        var pair = dbh.GetPairFromId(Id, user.Id);
                        if (dbh.DeletePair(pair))
                        {
                            var strMessage = CultureTextRequest.GetMessageString("cryptoPairRemoved", user.Language);
                            await BotApi.SendMessage(user.TelegramId, string.Format(strMessage, arg0: pair.TaskStatus(), arg1: pair.Id));
                        }
                        else
                        {
                            await BotApi.SendMessage(user.TelegramId,
                                string.Format(CultureTextRequest.GetMessageString("cryptoPairCantRemove", user.Language)));
                        }
                    }
                }
            }
        }
        public async void RemoveUserTask(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.DeletePair.Match(update.Message.Text);
            if (match.Success)
            {
                var user = BotApi.GetUserSettings(update).Result;
                var strId = match.Groups["id"].Value;
                if (!string.IsNullOrWhiteSpace(strId))
                {

                    int id = int.Parse(strId);
                    using (CryptoPairDbHandler dbh = new CryptoPairDbHandler())
                    {
                        var pair = dbh.GetPairFromId(id, user.Id);
                        var strMessage = dbh.DeletePair(pair)
                            ? string.Format(CultureTextRequest.GetMessageString("cryptoPairRemoved", user.Language), arg0: pair, arg1: pair.Id)
                            : string.Format(CultureTextRequest.GetMessageString("cryptoPairCantRemove", user.Language));

                        await BotApi.SendMessage(user.TelegramId, strMessage);

                    }
                }
                else
                {
                    var pairbase = match.Groups["base"].Value.ToUpper();
                    var pairquote = match.Groups["quote"].Value.ToUpper();
                    if (!string.IsNullOrWhiteSpace(pairbase) && !string.IsNullOrWhiteSpace(pairquote))
                    {
                        TradingPair pair = new TradingPair(pairbase, pairquote);
                        var strMessage = CultureTextRequest.GetMessageString("cryptoPairRemoveBySymbol", user.Language);
                        ListMatchingTasks(pair, user, CallbackDataPatterns.DeletePair, strMessage);
                    }
                    else if (!string.IsNullOrWhiteSpace(pairbase))
                    {
                        var strMessage = CultureTextRequest.GetMessageString("cryptoPairRemoveBySymbol", user.Language);
                        ListMatchingTasks(pairbase, user, CallbackDataPatterns.DeletePair, strMessage);
                    }
                    else
                    {
                        await BotApi.SendMessage(user.TelegramId, CultureTextRequest.GetMessageString("CPRemoveEmpty", user.Language));

                    }
                }
                //PairsManager.TempObjects?.Remove(pair);
            }
        }

        public async void SetSingleTriggerForUserTask(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.TriggerOncePair.Match(update.Message.Text);
            if (match.Success)
            {
                var id = int.Parse(match.Groups["id"].Value);
                var user = await BotApi.GetUserSettings(update);
                using (CryptoPairDbHandler dbHandler = new CryptoPairDbHandler())
                {
                    var pair = dbHandler.GetPairFromId(id, user.Id);
                    if (pair != null)
                    {
                        pair.TriggerOnce = !pair.TriggerOnce;
                        var trigger = pair.TriggerOnce ? "☑️ enabled" : "❌ disabled";
                        var msg = string.Format(
                            CultureTextRequest.GetMessageString("CPTriggerOnceChange", user.Language),
                            pair.TaskStatus(), trigger);
                        var editresult = await dbHandler.CompletlyEditCryptoPair(pair);
                        msg = editresult ? msg : "Can't save your changes because of unexpected error!";
                        await BotApi.SendMessage(user.TelegramId, msg, ParseMode.Html);
                    }
                    else
                    {
                        await BotApi.SendMessage(user.TelegramId, "Task not exists, or not yours!");
                    }
                }
            }
        }
        public async void AddCommentForTask(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.AddComment.Match(update.Message.Text);
            if (match.Success)
            {
                var id = int.Parse(match.Groups["id"].Value);
                var user = await BotApi.GetUserSettings(update);
                var task = new CryptoPairDbHandler().GetPairFromId(id, user.Id);
                var msg = task != null ? string.Format(CultureTextRequest.GetMessageString("CPAddComment", user.Language), task.Id, task)
                                             : "Task not exists, or not yours!";
                await BotApi.SendMessage(user.TelegramId, msg, true);
            }
        }
        public async void AddCommentForTaskReplyHandler(Update update)
        {
            var user = await BotApi.GetUserSettings(update);
            var msgRegex = CommandsRegex.ConvertMessageToRegex(CultureTextRequest.GetMessageString("CPAddComment", user.Language), new List<string>()
            {
                @"(?<id>[0-9]*)", @"(?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})"
            });
            var match = msgRegex.Match(update.Message?.ReplyToMessage?.Text);
            if (match.Success)
            {
                var id = int.Parse(match.Groups["id"].Value);
                var task = new CryptoPairDbHandler().GetPairFromId(id, user.Id);
                if (task != null)
                {
                    task.Note = update.Message.Text;
                }
            }
        }
        #endregion

        #region CreateTask stages
        /// <summary>
        /// Create task entry method. Uses for handle /create "base"/"quote" "price" command
        /// Can handle command without params, with symbol only and with symbol and price
        /// </summary>
        /// <param name="update">Latest message from user</param>
        /// <param name="user">UserConfig from db</param>
        public async void CreateTaskFirstStage(Update update, UserConfig user)
        {
            var match = CommandsRegex.MonitoringTaskCommands.CreatePair.Match(update.Message.Text);
            if (match.Success)
            {
                string baseValue = match.Groups["base"].Value.ToUpper();
                string quoteValue = match.Groups["quote"].Value.ToUpper();
                string priceStr = match.Groups["price"].Value;
                var pair = GetTempUserTask(update).Result;
                if (!String.IsNullOrWhiteSpace(baseValue) && !string.IsNullOrWhiteSpace(quoteValue))
                {
                    pair.PairBase = baseValue;
                    pair.PairQuote = quoteValue;
                    if (!string.IsNullOrWhiteSpace(priceStr))
                    {
                        pair.Price = double.Parse(priceStr, new CultureInfo("en"));
                    }
                    await SetExchangePStage(update, pair, user);
                }
                else
                {
                    await BotApi.SendMessage(update.Message.Chat.Id, Messages.newPairRequestingForPair, true, ParseMode.Html);
                }
            }
        }
        /// <summary>
        /// Creates new pair from user's message like: "base"/"quote".
        /// This method parses answer on replied message of creating new task without arguments
        /// </summary>
        /// <param name="update">Latest message from user, must contains only message with text "base"/"quote"</param>
        public async void SetPairSymbolStage(Update update, UserConfig user)
        {
            var match = RegexCombins.CryptoPairRegex.Match(update.Message?.Text);
            if (match.Success)
            {
                var pair = GetTempUserTask(update).Result;
                pair.PairBase = match.Groups["base"].Value.ToUpper();
                pair.PairQuote = match.Groups["quote"].Value.ToUpper();
                await SetExchangePStage(update, pair, user);
            }
            else
            {
                await BotApi.SendMessage(update.Message.Chat.Id, "Wrong pair name!");
            }
        }
        private async Task SetExchangePStage(Update update, CryptoPair pair, UserConfig user)
        {
            var exchangesForPair = await
                Program.cryptoData.GetExchangesForPair(
                    new CryptoApi.Objects.TradingPair(pair.PairBase, pair.PairQuote));
            if (exchangesForPair.Any())
            {
                if (user.SetExchangeAutomaticaly || exchangesForPair.Count == 1)
                {
                    pair.ExchangePlatform = exchangesForPair.First();
                    if (pair.Price != 0)
                        SetRaiseOrFallStage(update, pair);
                    else await BotApi.SendMessage(user.TelegramId,
                     "Exchange for new crypto pair setted. Set price in next message", true);
                }
                else
                {
                    var kbexchanges = Keyboards.ExchangeSelectingKeyboardMarkup(exchangesForPair);
                    await BotApi.SendMessage(user.TelegramId, $"Select crypto exchange for {pair}: ", kbexchanges);

                }
            }
            else
            {
                await BotApi.SendMessage(user.TelegramId, "Wrong pair!");

            }
        }


        public async Task SetExchangeCallbackHandlerStage(Update update)
        {
            var exchange = update.CallbackQuery?.Data;
            var pair = GetTempUserTask(update).Result;
            pair.ExchangePlatform = exchange;
            if (pair.Price == 0)
            {
                await BotApi.SendMessage(update.CallbackQuery?.From.Id,
                    "Exchange for new crypto pair setted. Set price in next message", true);
            }
            else if (pair.Price > 0)
            {
                SetRaiseOrFallStage(update, pair);
            }
        }

        /// <summary>
        /// Automatically sets direction of price trigger based on current price. Can set new price to task if not its set.
        /// </summary>
        private async void SetRaiseOrFallStage(Update update, CryptoPair pair, UserConfig user = default(UserConfig), double price = 0)
        {
            user = (user is null) ? await BotApi.GetUserSettings(update) : user;
            if (price > 0)
                pair.Price = price;
            if (string.IsNullOrWhiteSpace(pair.ToString()))
            {
                var lastMsgId = await BotApi.GetMessageIdFromUpdateTask(update);
                if (lastMsgId != 0)
                    await BotApi.EditMessage(user.TelegramId, lastMsgId, "Task creating expired. Start again");
                else await BotApi.SendMessage(user.TelegramId, "Task creating expired. Start again");
            }
            else
            {
                var curprice = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());
                pair.GainOrFall = curprice.Price < pair.Price;
                SaveNewTaskToDB(update, user);
            }
        }

        /// <summary>
        /// Accepts price value from user to set it to his task
        /// </summary>
        public async void SetPriceStage(Update update, UserConfig user)
        {
            var pair = GetTempUserTask(update).Result;
            try
            {
                var price = double.Parse(update.Message?.Text, new CultureInfo("en"));
                SetRaiseOrFallStage(update, pair, user, price);
            }
            catch (ArgumentException ex)
            {
                await BotApi.SendMessage(update.Message?.Chat?.Id, Messages.newPairWrongPrice, true);
            }
            catch (FormatException ex)
            {
                await BotApi.SendMessage(update.Message?.Chat?.Id, Messages.newPairWrongPrice, true);
            }
        }

        /// <summary>
        /// Last stage of creating tasks, saving to db and sends message to user, that is pair is created successfuly
        /// </summary>
        private async void SaveNewTaskToDB(Update update, UserConfig user)
        {
            var pair = GetTempUserTask(update).Result;
            if (pair?.Price > 0 &&
                !string.IsNullOrWhiteSpace(pair?.PairBase) &&
                !string.IsNullOrWhiteSpace(pair?.PairQuote) &&
                !string.IsNullOrWhiteSpace(pair?.ExchangePlatform))
            {
                using (AppDbContext db = new AppDbContext())
                {
                    pair.Enabled = true;
                    db.CryptoPairs.Add(pair);
                    PairsManager.TempObjects?.Remove(pair);
                    db.SaveChangesAsync();
                }

                var msg = CultureTextRequest.GetSettingsMsgString("CPEditTaskCreated", user.Language);
                var formatedmsg = $"{msg} {pair.FullTaskInfo(user.Language)}";

                BotApi.SendMessage(BotApi.GetTelegramIdFromUpdate(update).Identifier, formatedmsg, ParseMode.Html);
            }
        }
        #endregion

        #region DisplayTaskInfo

        public async void ShowTaskInfo(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.ShowPair.Match(update.Message.Text);
            if (match.Success)
            {
                var user = BotApi.GetUserSettings(update).Result;
                var strId = match.Groups["id"].Value;
                if (!string.IsNullOrWhiteSpace(strId))
                {
                    int id = int.Parse(strId);
                    using (CryptoPairDbHandler dbh = new CryptoPairDbHandler())
                    {
                        var pair = dbh.GetPairFromId(id, user.Id);
                        var pairCurrentPrice = await Program.cryptoData.GetCurrentPricePairByName(pair.ToTradingPair());

                        BotApi.SendMessage(user.TelegramId, pair.FullTaskInfo() + $"\nCurrent price: {pairCurrentPrice.Price} {pair.PairQuote}", ParseMode.Html);
                    }
                }
                else
                {
                    var pairbase = match.Groups["base"].Value.ToUpper();
                    var pairquote = match.Groups["quote"].Value.ToUpper();
                    if (!string.IsNullOrWhiteSpace(pairbase) && !string.IsNullOrWhiteSpace(pairquote))
                    {
                        TradingPair pair = new TradingPair(pairbase, pairquote);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(CultureTextRequest.GetMessageString("CPShowTasks", user.Language));
                        using (AppDbContext dbContext = new AppDbContext())
                        {
                            var pairs = dbContext.CryptoPairs.Where(x =>
                                x.OwnerId == user.Id && x.PairBase == pair.Name && x.PairQuote == pair.Quote).ToList();
                            foreach (var pairz in pairs)
                            {
                                sb.AppendLine(pairz.TaskStatusWithLink());
                            }
                        }
                        var pairCurrentPrice = await Program.cryptoData.GetCurrentPricePairByName(pairbase, pairquote);
                        BotApi.SendMessage(user.TelegramId, sb.ToString() + $"\nCurrent price: {pairCurrentPrice.Price} {pairquote}", ParseMode.Html);
                    }
                    else if (!string.IsNullOrWhiteSpace(pairbase))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(CultureTextRequest.GetMessageString("CPShowTasks", user.Language));
                        using (AppDbContext dbContext = new AppDbContext())
                        {
                            var pairs = dbContext.CryptoPairs.Where(x =>
                                x.OwnerId == user.Id && x.PairBase == pairbase).ToList();
                            foreach (var pairz in pairs)
                            {
                                sb.AppendLine(pairz.TaskStatusWithLink());
                            }
                        }
                        var pairCurrentPrice = await Program.cryptoData.GetCurrentPricePairByName(pairbase, "USDT");
                        BotApi.SendMessage(user.TelegramId, sb.ToString() + $"\nCurrent price: {pairCurrentPrice.Price} USDT", ParseMode.Html);
                    }
                    else
                    {
                        // BotApi.SendMessage(user.TelegramId, CultureTextRequest.GetMessageString("CPRemoveEmpty", user.Language));
                    }
                }
                //PairsManager.TempObjects?.Remove(pair);
            }
        }

        public async void ListMatchingTasks(TradingPair pair, UserConfig user, string datapattern, string Message)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var tasks = dbContext.CryptoPairs.Where(x => x.OwnerId == user.Id && x.PairBase == pair.Name && x.PairQuote == pair.Quote).ToList();
                if (tasks != null && tasks.Any())
                {
                    var kb = Keyboards.PairsSelectingKeyboardMarkup(tasks, datapattern);
                    BotApi.SendMessage(user.TelegramId, Message, replyMarkup: kb);
                }
                else BotApi.SendMessage(user.TelegramId, CultureTextRequest.GetMessageString("CPCantFindAnyPairsMatching", user.Language));
            }
        }

        public async void ListMatchingTasks(string SymbolBase, UserConfig user, string datapattern, string Message)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var tasks = dbContext.CryptoPairs.Where(x => x.OwnerId == user.Id && x.PairBase == SymbolBase).ToList();
                if (tasks != null && tasks.Any())
                {
                    var kb = Keyboards.PairsSelectingKeyboardMarkup(tasks, datapattern);
                    BotApi.SendMessage(user.TelegramId, Message, replyMarkup: kb);
                }
                else
                {
                    BotApi.SendMessage(user.TelegramId,
                        CultureTextRequest.GetMessageString("CPCantFindAnyPairsMatching", user.Language));
                }
            }
        }

        public async void ListAllTask(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                if (user != null)
                {
                    var tasks = dbContext.CryptoPairs.Where(x => x.OwnerId == user.Id).ToList();
                    if (tasks.Any())
                    {
                        StringBuilder strTasks = new StringBuilder("Your list of tasks:\n");
                        foreach (var task in tasks)
                        {
                            strTasks.AppendLine(task.TaskStatusWithLink());
                        }

                        strTasks.AppendLine("");
                        strTasks.AppendLine("To edit any task send: /edit 'id' 'new_price' or /edit BASE/QUOTE");
                        strTasks.AppendLine("*💎 - single trigger tasks, 🌗 - that trigger fired");
                        BotApi.SendMessage(update.Message.Chat.Id, strTasks.ToString(), ParseMode.Html);
                    }
                    else BotApi.SendMessage(update.Message.Chat.Id, CultureTextRequest.GetMessageString("noCryptoTasks", user.Language));
                }
                else BotApi.SendMessage(update.Message.Chat.Id, CultureTextRequest.GetMessageString("noCryptoTasks", "en"));
            }
        }


        #endregion
        #region MultiTasksOperations
        public async void DropEverythingByProcent(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.ShiftTasks.Match(update.Message.Text);
            if (match.Success)
            {
                var user = await BotApi.GetUserSettings(update);
                var procent = !string.IsNullOrWhiteSpace(match.Groups["procent"].Value) ? int.Parse(match.Groups["procent"].Value) : 2;
                var isCreateAltTasks = !string.IsNullOrWhiteSpace(match.Groups["create"].Value);

                var appDbContext = new AppDbContext();
                var pairs = await MonitorLoop.UserTasksToNotify(user, appDbContext, false);
                foreach (var pair in pairs)
                {
                    if (isCreateAltTasks)
                    {
                        CryptoPair cp = pair.Item1.Clone() as CryptoPair;
                        cp.Id = 0;
                        cp.GainOrFall = !cp.GainOrFall;
                        appDbContext.CryptoPairs.Add(cp);
                    }
                    if (pair.Item1.GainOrFall && pair.Item1.Price < pair.Item2)
                        pair.Item1.Price = pair.Item2 * (procent + 100) / 100;
                    else if (!pair.Item1.GainOrFall && pair.Item1.Price > pair.Item2)
                        pair.Item1.Price = pair.Item2 * (100 - procent) / 100;
                }
                appDbContext.SaveChangesAsync();
                BotApi.SendMessage(user.TelegramId, "Saved!");

            }
        }

        public async void FlipTriggeredTasks(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                var pairs = await MonitorLoop.UserTasksToNotify(user, dbContext, false);
                StringBuilder sb = new StringBuilder();
                foreach (var pair in pairs)
                {
                    pair.Item1.GainOrFall = !pair.Item1.GainOrFall;
                    sb.AppendLine($"Trigger flipped for task id {pair.Item1.Id} {pair.Item1.TaskStatus()}");
                }
                dbContext.SaveChangesAsync();
                BotApi.SendMessage(user.TelegramId, sb.ToString());
            }
        }
        #endregion


    }
}
