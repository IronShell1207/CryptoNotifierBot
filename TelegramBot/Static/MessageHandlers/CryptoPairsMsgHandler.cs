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
        private void RemoveTempUserTask(CryptoPair pair)
        {
            PairsManager.TempObjects?.Remove(pair);
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
            await BotApi.SendMessage(update.CallbackQuery.From.Id,
                "Exchange for new crypto pair setted. Set price in next message", true);
        }

        public async void SetTriggerPriceStageCP(Update update)
        {
            var pair = GetTempUserTask(update).Result;
            try
            {
                var price = double.Parse(update.Message.Text);
                pair.Price = price;
                if (pair.ToString() == "/")
                    BotApi.SendMessage(update.Message.Chat.Id, "Task creating expired. Start again");
                else
                {
                    var curprice = ExchangesCheckerForUpdates.GetCurrentPrice(new TradingPair(
                        pair.PairBase, pair.PairQuote), pair.ExchangePlatform);
                    pair.GainOrFall = curprice.Result < price;
                    SaveNewTaskToDB(update);
                }
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

                BotApi.SendMessage(update.Message.Chat.Id, "New pair saved!");
            }
        }

        public async void ListAllTask(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var tasks = dbContext.CryptoPairs.Where(x => x.OwnerId == update.Message.Chat.Id).ToList();
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
                else BotApi.SendMessage(update.Message.Chat.Id, MessagesGetter.GetGlobalString("noCryptoTasks", "en"));
            }
        }
    }
}
