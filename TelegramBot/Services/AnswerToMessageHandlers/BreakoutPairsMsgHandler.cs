using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoApi.API;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Constants.Commands;
using TelegramBot.Helpers;
using TelegramBot.Objects;

namespace TelegramBot.Static.MessageHandlers
{
    public class BreakoutPairsMsgHandler : IMyDisposable
    {
        /// <summary>
        /// Остановить подписку.
        /// </summary>
        /*public async Task StopNotify(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.BreakoutSubs.ToList().FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                if (user.Subscribed)
                {
                    user.Subscribed = false;
                    await db.SaveChangesAsync();
                    await BotApi.SendMessage(user.TelegramId, "Your breakouts subscription deactivated. To start subscription send /subscribe");
                }
            }
        }

        /// <summary>
        /// Подписаться на обновления
        /// </summary>
        /// <param name="update"></param>
        public async Task SubNewUserBreakouts(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var user = await BotApi.GetUserSettings(update);
                var sub = db.BreakoutSubs?.OrderBy(x => x.Id).FirstOrDefault(x => x.TelegramId == user.TelegramId);
                if (sub == null)
                {
                    sub = new BreakoutSub()
                    {
                        Subscribed = true,
                        TelegramId = user.TelegramId,
                        BinanceSub = true,
                        BitgetSub = true,
                        BlackListEnable = false,
                        GateioSub = false,
                        KucoinSub = true,
                        OkxSub = true,
                        UserId = user.Id
                    };
                    db.BreakoutSubs?.Add(sub);
                    await db.SaveChangesAsync();
                    await BotApi.SendMessage(update.Message.Chat.Id, string.Format(Messages.subscribedSucs,
                        $"\nGate IO platform: {sub.GateioSub}\nBinance platform: {sub.BinanceSub}\nOkx platfrom: {sub.OkxSub}\nKucoin platform: {sub.KucoinSub}"));
                }
                else if (sub.Subscribed)
                {
                    await BotApi.SendMessage(update.Message.Chat.Id, "You already subscribed to breakout bot. Your current settings: " +
                                                                     $"\nGate IO platform: {sub.GateioSub}\nBinance platform: {sub.BinanceSub}" +
                                                                     $"\nOkx platfrom: {sub.OkxSub}\nKucoin platform: {sub.KucoinSub}");
                }
                else if (!sub.Subscribed)
                {
                    sub.Subscribed = true;
                    await db.SaveChangesAsync();
                    await BotApi.SendMessage(update.Message.Chat.Id, string.Format(Messages.subscribedSucs,
                        $"\nGate IO platform: {sub.GateioSub}\nBinance platform: {sub.BinanceSub}" +
                        $"\nOkx platfrom: {sub.OkxSub}\nKucoin platform: {sub.KucoinSub}"));
                }
            }
        }

        /// <summary>
        /// Установить временные рамки обновлений.
        /// </summary>
        public async Task SetTimings(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var sub = db.BreakoutSubs.FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                var match = BreakoutCommands.SetTimings.Match(update.Message?.Text);
                var timing = double.Parse(match.Groups["timing"].Value, new CultureInfo("en"));
                var enable = false;
                switch (timing)
                {
                    case 2:
                        sub.S2MinUpdates = !sub.S2MinUpdates;
                        enable = sub.S2MinUpdates;
                        break;

                    case 5:
                        sub.S5MinUpdates = !sub.S5MinUpdates;
                        enable = sub.S5MinUpdates;
                        break;

                    case 15:
                        sub.S15MinUpdates = !sub.S15MinUpdates;
                        enable = sub.S15MinUpdates;
                        break;

                    case 30:
                        sub.S30MinUpdates = !sub.S30MinUpdates;
                        enable = sub.S30MinUpdates;
                        break;

                    case 45:
                        sub.S45MinUpdates = !sub.S45MinUpdates;
                        enable = sub.S45MinUpdates;
                        break;

                    case 60:
                        sub.S60MinUpdates = !sub.S60MinUpdates;
                        enable = sub.S60MinUpdates;
                        break;

                    case 120:
                        sub.S120MinUpdates = !sub.S120MinUpdates;
                        enable = sub.S120MinUpdates;
                        break;

                    case 240:
                        sub.S240MinUpdates = !sub.S240MinUpdates;
                        enable = sub.S240MinUpdates;
                        break;

                    case 480:
                        sub.S480MinUpdates = !sub.S480MinUpdates;
                        enable = sub.S480MinUpdates;
                        break;

                    case 960:
                        sub.S960MinUpdates = !sub.S960MinUpdates;
                        enable = sub.S960MinUpdates;
                        break;

                    case 1920:
                        sub.S1920MinUpdates = !sub.S1920MinUpdates;
                        enable = sub.S1920MinUpdates;
                        break;
                }

                var en = enable ? "enabled" : "disabled";
                await BotApi.SendMessage(sub.TelegramId, $"Updates for {timing} mins {en}!");
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public async Task SubSettings(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var subber = db.BreakoutSubs.ToList().Find(x => x.TelegramId == update.Message.Chat.Id);
                if (subber != null)
                {
                    var message = $"{21}";
                    await BotApi.SendMessage(update.Message.Chat.Id, message, true);
                }
            }
        }

        /// <summary>
        /// Добавить пару в черный/белый список.
        /// </summary>
        public async Task AddPairToBlackListCommandHandler(Update update)
        {
            var match = BreakoutCommands.AddToBlackList.Match(update.Message.Text);
            if (match.Success)
            {
                var Pairbase = match.Groups["base"].Value.ToUpper();
                var Pairquote = match.Groups["quote"].Value.ToUpper();
                if (!string.IsNullOrWhiteSpace(Pairbase) && !string.IsNullOrWhiteSpace(Pairquote))
                    AddPairToBlackList(update, new TradingPair(Pairbase, Pairquote));
                else
                {
                    await BotApi.SendMessage(update.Message.Chat.Id,
                        CultureTextRequest.GetMessageString("ToaddToTheBlackList", "en"),
                        true);
                }
            }
            else if (RegexCombins.CryptoPairRegex.IsMatch(update.Message.Text.ToUpper()))
            {
                match = RegexCombins.CryptoPairRegex.Match(update.Message.Text.ToUpper());
                var Pairbase = match.Groups["base"].Value;
                var Pairquote = match.Groups["quote"].Value;
                if (!string.IsNullOrWhiteSpace(Pairbase) && !string.IsNullOrWhiteSpace(Pairquote))
                    AddPairToBlackList(update, new TradingPair(Pairbase, Pairquote));
            }
        }

        /// <summary>
        /// Добавить пару в черный список.
        /// </summary>
        public async void AddPairToBlackList(Update update, TradingPair pair)
        {
            var user = await BotApi.GetUserSettings(update);
            var isValid = Program.cryptoData.GetCurrentPricePairByName(pair);
            if (isValid != null)
            {
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var breakoutSetts = dbContext.BreakoutSubs.Include(x => x.BlackListedPairsList).OrderBy(x => x.Id)
                        .FirstOrDefault(x => x.UserId == user.Id);
                    if (breakoutSetts == null)
                    {
                        await BotApi.SendMessage(user.TelegramId, "You're not subscribed to breakouts notifications. To subscribe send /subscribe");
                        return;
                    }
                    var existsPairCheck =
                        breakoutSetts.BlackListedPairsList?.FirstOrDefault(x => x.ToString() == pair.ToString());
                    if (existsPairCheck != null)
                    {
                        await BotApi.SendMessage(user.TelegramId, string.Format(
                                            CultureTextRequest.GetMessageString("blacklistPairExists", user.Language), pair.ToString()));
                        return;
                    }
                    else
                    {
                        breakoutSetts.BlackListedPairsList?.Add(new BlackListedPairs(pair));
                        await dbContext.SaveChangesAsync();
                        await BotApi.SendMessage(user.TelegramId, string.Format(
                                      CultureTextRequest.GetMessageString("blacklistPairAdded", user.Language), pair.ToString()));
                    }
                }
            }
            else
            {
                await BotApi.SendMessage(user.TelegramId, "Pair doesnt exists!");
            }
        }

        public async Task<CommandHandlerResult> RemoveAllBlackListedPairsUser(UserConfig user)
        {
            using (var dbcontext = new AppDbContext())
            {
                var breakoutUsrCfg = dbcontext.BreakoutSubs.Include(x => x.BlackListedPairsList).First(x => x.TelegramId == user.TelegramId);
                breakoutUsrCfg.BlackListedPairsList?.Clear();
                await dbcontext.SaveChangesAsync();
                return new CommandHandlerResult("Success", true);
            }
        }

        public async Task AddWhiteTopList(Update update)
        {
            var match = BreakoutCommands.AddTopSymbolsToWhiteList.Match(update.Message.Text);
            if (match.Success)
            {
                var count = match.Groups["count"].Value != null ? int.Parse(match.Groups["count"].Value) : 0;
                var topPairs = await new CoinMarketCapTop().GetCmcTopPairs(count);
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var userBreakoutCfg = dbContext.BreakoutSubs.OrderBy(x => x.TelegramId).
                        FirstOrDefault(x => x.TelegramId == update.Message.From.Id);
                    if (userBreakoutCfg != null)
                    {
                        userBreakoutCfg.WhitelistInsteadBlack = true;
                        userBreakoutCfg.BlackListEnable = true;
                        userBreakoutCfg.BlackListedPairsList?.RemoveAll(x => x.OwnerId == userBreakoutCfg.Id);
                        foreach (var pair in topPairs)
                        {
                            BlackListedPairs bpair = new BlackListedPairs(pair);
                            userBreakoutCfg.BlackListedPairsList?.Add(bpair);
                        }

                        await dbContext.SaveChangesAsync();
                        await BotApi.SendMessage(update.Message.From.Id,
                            $"White listed pairs saved to the db in count of {topPairs.Count}");
                    }
                    else await BotApi.SendMessage(update.Message.From.Id, "You're not subscribed!");
                }
            }
        }*/
    }
}