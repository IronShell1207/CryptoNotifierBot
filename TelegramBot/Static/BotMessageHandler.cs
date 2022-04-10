using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Objects;
using CryptoPair = TelegramBot.Objects.CryptoPair;

namespace TelegramBot.Static
{
    public class BotMessageHandler : IDisposable
    {
        private bool disposed = false;
        public async void AddCryptoPair(Update update)
        {
            var match = CommandsRegex.CreatePair.Match(update.Message.Text);
            if (match.Success)
            {
                var user = await BotApi.GetUserSettings(update.Message.Chat.Id);
                string baseValue = match.Groups["base"].Value.ToUpper();
                string quoteValue = match.Groups["quote"].Value.ToUpper();
                string priceStr = match.Groups["price"].Value;
                CryptoPair pair = PairsManager.TempObjects?.FirstOrDefault(x => x.OwnerId == user.Id);
                if (pair == null)
                    pair = new CryptoPair(user.Id);
                PairsManager.TempObjects.Add(pair);
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

                    var kbexchanges = Keyboards.ExchangeSelectingKeyboardMarkup(exchangesForPair.Result);
                    BotApi.SendMessage(update.Message.Chat.Id, "Select crypto exchanges for your pair: ", kbexchanges);

                }
            }
        }

        public async void SelectingCryptoExchangeForPairQueryHandler(Update update)
        {
            var user = await BotApi.GetUserSettings(update.CallbackQuery.From.Id);
            CryptoPair pair = PairsManager.TempObjects?.FirstOrDefault(x => x.OwnerId == user.Id);
            if (pair == null)
            {
                BotApi.SendMessage(user.TelegramId, "Task creating invoice expired!");
                return;
            }
            pair.ExchangePlatform = update.CallbackQuery.Data;
            if (pair.Price == 0)
            {
                BotApi.SendMessage(user.TelegramId, $"Now set a trigger price for pair: {pair.ToString()}");
                return;
            }

            BotApi.SendMessage(user.TelegramId, "");
        }

        public async void SettingPriceForNewTask(Update update)
        {
            var user = await BotApi.GetUserSettings(update.Message.Chat.Id);

        }

        public async void SubNewUserBreakouts(Update update)
        {
            var user = new BreakoutSub()
            {
                TelegramId = update.Message.Chat.Id
            };
            using (AppDbContext db = new AppDbContext())
            {
                if (db.BreakoutSubs.ToList().FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id) == null)
                {
                    db.BreakoutSubs.Add(user);
                    db.SaveChangesAsync();
                    BotApi.SendMessage(update.Message.Chat.Id, string.Format(Messages.subscribedSucs,
                        $"\nGate IO platform: {user.GateioSub}\nBinance platform: {user.BinanceSub}\nOkx platfrom: {user.OkxSub}\nKucoin platform: {user.KucoinSub}"));
                }
                BotApi.SendMessage(update.Message.Chat.Id, "You already subscribed to breakout bot. Your current settings: " +
                    $"\nGate IO platform: {user.GateioSub}\nBinance platform: {user.BinanceSub}\nOkx platfrom: {user.OkxSub}\nKucoin platform: {user.KucoinSub}");
            }
        }

        public async void SubSettings(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var subber = db.BreakoutSubs.ToList().Find(x => x.TelegramId == update.Message.Chat.Id);
                if (subber != null)
                {
                    var message = $"{21}";
                    BotApi.SendMessage(update.Message.Chat.Id, message, true);
                }
            }
        }








        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                if (disposing)
                {

                }

                disposed = true;
            }

        }
    }
}
