using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Objects;
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

        public void EditUserTask(Update update)
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
                            var msg = string.Format(MessagesGetter.GetGlobalString("CPEditPair", user.Language), task.TaskStatus());
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
                            var strMessage = MessagesGetter.GetGlobalString("cryptoPairRemoved", user.Language);
                            BotApi.SendMessage(user.TelegramId, string.Format(strMessage, arg0: pair.ToString(), arg1: pair.Id));
                        }
                        else
                        {
                            BotApi.SendMessage(user.TelegramId,
                                string.Format(MessagesGetter.GetGlobalString("cryptoPairCantRemove", user.Language)));
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
                            var strMessage = MessagesGetter.GetGlobalString("cryptoPairRemoved", user.Language);
                            BotApi.SendMessage(user.TelegramId, string.Format(strMessage, arg0: pair.ToString(), arg1: pair.Id));
                        }
                        else
                        {
                            BotApi.SendMessage(user.TelegramId,
                                string.Format(MessagesGetter.GetGlobalString("cryptoPairCantRemove", user.Language)));
                        }
                    }
                }
                else
                {
                    TradingPair pair = new TradingPair(match.Groups["base"].Value.ToUpper(),
                        match.Groups["quote"].Value.ToUpper());

                    if (!string.IsNullOrWhiteSpace(pair.ToString()))
                    {
                        var strMessage = MessagesGetter.GetGlobalString("cryptoPairRemoveBySymbol", user.Language);
                        ListMatchingTasks(pair, user, CallbackDataPatterns.DeletePair, strMessage);
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
                        pair.Price = double.Parse(priceStr);
                    }

                    var exchangesForPair =
                        ExchangesCheckerForUpdates.GetExchangesForPair(
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
                    ExchangesCheckerForUpdates.GetExchangesForPair(
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

        public async void SetExchangeStageCP(Update update)
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
            if (price > 0)
                pair.Price = price;
            if (pair.ToString() == "/")
                BotApi.SendMessage(update.Message.Chat.Id, "Task creating expired. Start again");
            else
            {
                var curprice = ExchangesCheckerForUpdates.GetCurrentPrice(new TradingPair(
                    pair.PairBase, pair.PairQuote), pair.ExchangePlatform);
                pair.GainOrFall = curprice.Result < pair.Price;
                SaveNewTaskToDB(update);
            }
        }

        public async void SetTriggerPriceStageCP(Update update)
        {
            var pair = GetTempUserTask(update).Result;
            try
            {
                var price = double.Parse(update.Message.Text);
                SetRaiseOrFallStatus(update, pair, price);

            }
            catch (ArgumentException ex)
            {
                BotApi.SendMessage(update.Message.Chat.Id, Messages.newPairWrongPrice, true);
            }
        }

        private async void SaveNewTaskToDB(Update update)
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
                    db.SaveChangesAsync();
                }

                BotApi.SendMessage(BotApi.GetTelegramIdFromUpdate(update).Identifier, "New pair saved!");
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
                else BotApi.SendMessage(user.TelegramId, MessagesGetter.GetGlobalString("CPCantFindAnyPairsMatching", user.Language));
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
                            strTasks.AppendLine(task.TaskStatus());
                        }

                        strTasks.AppendLine("");
                        strTasks.AppendLine("To edit any task send: /edit 'id' 'new_price' or /edit BASE/QUOTE");
                        BotApi.SendMessage(update.Message.Chat.Id, strTasks.ToString());
                    }
                    else BotApi.SendMessage(update.Message.Chat.Id, MessagesGetter.GetGlobalString("noCryptoTasks", user.Language));
                }
                else BotApi.SendMessage(update.Message.Chat.Id, MessagesGetter.GetGlobalString("noCryptoTasks", "en"));
            }
        }
    }
}
