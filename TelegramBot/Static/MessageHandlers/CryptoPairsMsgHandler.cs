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
            var userId = update.Message?.Chat?.Id ?? update.CallbackQuery?.From?.Id;
            var user = await BotApi.GetUserSettings(userId);
            CryptoPair pair = PairsManager.TempObjects?.FirstOrDefault(x => x.OwnerId == user.Id);
            if (pair == null)
            {
                pair = new CryptoPair(user.Id);
                PairsManager.TempObjects.Add(pair);
            }
            return pair;
        }

        public async void EditUserTask(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.EditPair.Match(update.Message.Text);
            var user = BotApi.GetUserSettings(BotApi.GetTelegramIdFromUpdate(update)).Result;
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

        }
        public void EditUserTaskCallbackHandler(Update update)
        {
            var match = CallbackDataPatterns.EditPairRegex.Match(update.CallbackQuery.Data);
            var user = BotApi.GetUserSettings(BotApi.GetTelegramIdFromUpdate(update)).Result;
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
                            var msg = string.Format(CultureTextRequest.GetMessageString("CPEditPair", user.Language), pair.ToString(), pair.Id);
                            BotApi.SendMessage(user.TelegramId, msg, true);
                        }

                    }
                }
            }
        }
        public void RemoveUserTaskCallbackHandler(Update update)
        {
            var match = CallbackDataPatterns.DeletePairRegex.Match(update.CallbackQuery.Data);
            var user = BotApi.GetUserSettings(BotApi.GetTelegramIdFromUpdate(update)).Result;
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
                            BotApi.SendMessage(user.TelegramId, string.Format(strMessage, arg0: pair.TaskStatus(), arg1: pair.Id));
                        }
                        else
                        {
                            BotApi.SendMessage(user.TelegramId,
                                string.Format(CultureTextRequest.GetMessageString("cryptoPairCantRemove", user.Language)));
                        }
                    }
                }
            }
        }
        public void RemoveUserTask(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.DeletePair.Match(update.Message.Text);
            if (match.Success)
            {
                var user = BotApi.GetUserSettings(update.Message.Chat.Id).Result;
                var strId = match.Groups["id"].Value;
                if (!string.IsNullOrWhiteSpace(strId))
                {

                    int id = int.Parse(strId);
                    using (CryptoPairDbHandler dbh = new CryptoPairDbHandler())
                    {
                        var pair = dbh.GetPairFromId(id, user.Id);
                        if (dbh.DeletePair(pair))
                        {
                            var strMessage = CultureTextRequest.GetMessageString("cryptoPairRemoved", user.Language);
                            BotApi.SendMessage(user.TelegramId, string.Format(strMessage, arg0: pair.ToString(), arg1: pair.Id));
                        }
                        else
                        {
                            BotApi.SendMessage(user.TelegramId,
                                string.Format(CultureTextRequest.GetMessageString("cryptoPairCantRemove", user.Language)));
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
                        BotApi.SendMessage(user.TelegramId, CultureTextRequest.GetMessageString("CPRemoveEmpty", user.Language));

                    }
                }
                //PairsManager.TempObjects?.Remove(pair);
            }
        }

        public void ShowTaskInfo(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.ShowPair.Match(update.Message.Text);
            if (match.Success)
            {
                var user = BotApi.GetUserSettings(update.Message.Chat.Id).Result;
                var strId = match.Groups["id"].Value;
                if (!string.IsNullOrWhiteSpace(strId))
                {
                    int id = int.Parse(strId);
                    using (CryptoPairDbHandler dbh = new CryptoPairDbHandler())
                    {
                        var pair = dbh.GetPairFromId(id, user.Id);
                        BotApi.SendMessage(user.TelegramId, pair.FullTaskInfo(), ParseMode.Html);
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
                        BotApi.SendMessage(user.TelegramId, sb.ToString(), ParseMode.Html);
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
                        BotApi.SendMessage(user.TelegramId, sb.ToString(), ParseMode.Html);
                    }
                    else
                    {
                       // BotApi.SendMessage(user.TelegramId, CultureTextRequest.GetMessageString("CPRemoveEmpty", user.Language));
                    }
                }
                //PairsManager.TempObjects?.Remove(pair);
            }
        }

        public async void NewCP(Update update)
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

                    var exchangesForPair =
                        Program.cryptoData.GetExchangesForPair(
                            new CryptoApi.Objects.TradingPair(pair.PairBase, pair.PairQuote));
                    if (exchangesForPair.Result.Any())
                    {
                        var kbexchanges = Keyboards.ExchangeSelectingKeyboardMarkup(exchangesForPair.Result);
                        await BotApi.SendMessage(update.Message.Chat.Id, "Select crypto exchanges for your pair: ", kbexchanges);
                    }
                    else await BotApi.SendMessage(update.Message.Chat.Id, "Wrong pair!");
                }
                else
                {
                    await BotApi.SendMessage(update.Message.Chat.Id, Messages.newPairRequestingForPair, true);
                }
            }
        }

        public async void CreatingPairStageCP(Update update)
        {
            var match = RegexCombins.CryptoPairRegex.Match(update.Message.Text);
            if (match.Success)
            {
                var pair = GetTempUserTask(update).Result;
                pair.PairBase = match.Groups["base"].Value.ToUpper();
                pair.PairQuote = match.Groups["quote"].Value.ToUpper();
                var exchangesForPair =
                    Program.cryptoData.GetExchangesForPair(
                        new CryptoApi.Objects.TradingPair(pair.PairBase, pair.PairQuote));
                if (exchangesForPair.Result.Any())
                { 
                    var kbexchanges = Keyboards.ExchangeSelectingKeyboardMarkup(exchangesForPair.Result);
                    await BotApi.SendMessage(update.Message.Chat.Id, "Select crypto exchanges for your pair: ", kbexchanges);
                }
                else await BotApi.SendMessage(update.Message.Chat.Id, "Wrong pair!");
            }
            else
            {
                await BotApi.SendMessage(update.Message.Chat.Id, "Wrong pair!");
            }
        }

        public async Task SetExchangeStageCP(Update update)
        {
            var exchange = update.CallbackQuery.Data;
            var pair = GetTempUserTask(update).Result;
            pair.ExchangePlatform = exchange;
            if (pair.Price == 0)
            {
                await BotApi.SendMessage(update.CallbackQuery.From.Id,
                    "Exchange for new crypto pair setted. Set price in next message", true);
            }
            else if (pair.Price > 0)
            {
                SetRaiseOrFallStatus(update, pair);
            }
        }

        private async void SetRaiseOrFallStatus(Update update, CryptoPair pair, double price = 0)
        {
            var user = BotApi.GetUserSettings(update).Result;
            if (price > 0)
                pair.Price = price;
            if (pair.ToString() == "/")
            {
                BotApi.EditMessage(update.Message.Chat.Id, BotApi.GetMessageIdFromUpdateTask(update).Result,
                    "Task creating expired. Start again");
                // BotApi.SendMessage(update.Message.Chat.Id, );
            }
            else
            {
                var curprice = await Program.cryptoData.GetCurrentPricePairByName(new TradingPair(pair.PairBase, pair.PairQuote), pair.ExchangePlatform);
                pair.GainOrFall = curprice.Price < pair.Price;
                SaveNewTaskToDB(update, user);
            }
        }

        public async void SetTriggerPriceStageCP(Update update)
        {
            var pair = GetTempUserTask(update).Result;
            try
            {
                var price = double.Parse(update.Message.Text, new CultureInfo("en"));
                SetRaiseOrFallStatus(update, pair, price);

            }
            catch (ArgumentException ex)
            {
                BotApi.SendMessage(update.Message.Chat.Id, Messages.newPairWrongPrice, true);
            }
        }

        private async void SaveNewTaskToDB(Update update, UserConfig user)
        {
            var pair = GetTempUserTask(update).Result;
            if (pair?.Price != null &&
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
                        BotApi.SendMessage(update.Message.Chat.Id, strTasks.ToString(), ParseMode.Html);
                    }
                    else BotApi.SendMessage(update.Message.Chat.Id, CultureTextRequest.GetMessageString("noCryptoTasks", user.Language));
                }
                else BotApi.SendMessage(update.Message.Chat.Id, CultureTextRequest.GetMessageString("noCryptoTasks", "en"));
            }
        }

        public async void DropEverythingByProcent(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.ShiftTasks.Match(update.Message.Text);
            if (match.Success)
            {
                var user = await BotApi.GetUserSettings(update);
                var procent = !string.IsNullOrWhiteSpace(match.Groups["procent"].Value) ? int.Parse(match.Groups["procent"].Value) : 2;
                var appDbContext = new AppDbContext();
                var pairs = await MonitorLoop.UserTasksToNotify(user, appDbContext, false);
                foreach (var pair in pairs)
                {
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

        public async void AddCommentForTask(Update update)
        {
            var match = CommandsRegex.MonitoringTaskCommands.AddComment.Match(update.Message.Text);
            if (match.Success)
            {
                var id = int.Parse(match.Groups["id"].Value);
                var user = await BotApi.GetUserSettings(update);
                var task = new CryptoPairDbHandler().GetPairFromId(id, user.Id);
                if (task != null)
                {
                    var msg = string.Format(CultureTextRequest.GetMessageString("CPAddComment", user.Language), task.Id, task.ToString());
                    BotApi.SendMessage(user.TelegramId, msg, true);
                }
                else
                {
                    BotApi.SendMessage(user.TelegramId, "Task not exists, or not yours!");
                }
            }
        }

        public async void AddCommentForTaskReplyHandler(Update update)
        {
            var user = await BotApi.GetUserSettings(update);
            var msgRegex = CommandsRegex.ConvertMessageToRegex(CultureTextRequest.GetMessageString("CPAddComment", user.Language), new List<string>()
            {
                @"(?<id>[0-9]*)", @"(?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})"
            });
            var match = msgRegex.Match(update.Message.ReplyToMessage.Text);
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
    }
}
