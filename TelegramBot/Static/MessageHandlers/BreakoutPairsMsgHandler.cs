using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.Constants;
using TelegramBot.Objects;

namespace TelegramBot.Static.MessageHandlers
{
    public class BreakoutPairsMsgHandler : IMyDisposable
    {
        public void StopNotify(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var user = db.BreakoutSubs.ToList().FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                if (user.Subscribed)
                {
                    user.Subscribed = false;
                    db.SaveChangesAsync();
                    BotApi.SendMessage(user.TelegramId, "Your breakouts subscription deactivated. To start subscription send /subscribe");
                }
            }
        }
        public async void SubNewUserBreakouts(Update update)
        {
            
            using (AppDbContext db = new AppDbContext())
            {
                var user = await BotApi.GetUserSettings(update);
                var sub = db.BreakoutSubs?.OrderBy(x => x.Id).FirstOrDefault(x =>x.TelegramId == user.TelegramId);
                if (sub == null)
                {
                    sub = new BreakoutSub()
                    {
                        Subscribed = true,
                        TelegramId = user.TelegramId,
                        BinanceSub = true,
                        BitgetSub = true,
                        BlackListEnable = false,
                        GateioSub =false,
                        KucoinSub = true,
                        OkxSub = true
                    };
                    db.BreakoutSubs.Add(sub);
                    db.SaveChangesAsync();
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
                    db.SaveChangesAsync();
                    await BotApi.SendMessage(update.Message.Chat.Id, string.Format(Messages.subscribedSucs,
                        $"\nGate IO platform: {sub.GateioSub}\nBinance platform: {sub.BinanceSub}" +
                        $"\nOkx platfrom: {sub.OkxSub}\nKucoin platform: {sub.KucoinSub}"));
                }

            }
        }

        public async void SetTimings(Update update)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var sub = db.BreakoutSubs.FirstOrDefault(x => x.TelegramId == update.Message.Chat.Id);
                var match = CommandsRegex.SetTimings.Match(update.Message.Text);
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
                BotApi.SendMessage(sub.TelegramId, $"Updates for {timing} mins {en}!");
                db.SaveChangesAsync();
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

        public async void AddPairToBlackListCommandHandler(Update update)
        {
            var match = CommandsRegex.BreakoutCommands.AddToBlackList.Match(update.Message.Text);
            if (match.Success)
            {
                var Pairbase = match.Groups["base"].Value.ToUpper();
                var Pairquote = match.Groups["quote"].Value.ToUpper();
                if (!string.IsNullOrWhiteSpace(Pairbase) && !string.IsNullOrWhiteSpace(Pairquote))
                    AddPairToBlackList(update, Pairbase, Pairquote);
                else
                {
                    BotApi.SendMessage(update.Message.Chat.Id,
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
                    AddPairToBlackList(update, Pairbase, Pairquote);
            }
            
        }

        public async void AddPairToBlackList(Update update, string Pairbase, string Pairquote)
        {
           //bool isValid = ExchangesCheckerForUpdates.GetCurrentPrice(new TradingPair(Pairbase, Pairquote)).;
            //if (isValid)
            //{
            //    var user = BotApi.GetUserSettings(update.Message.Chat.Id).Result;
            //    using (AppDbContext dbContext = new AppDbContext())
            //    {
            //        var blackPairEx = dbContext.BlackListedPairs.FirstOrDefault(x =>
            //            x.Base == Pairbase && x.Quote == Pairquote && x.OwnerId == user.Id);
            //        if (blackPairEx == null)
            //        {
            //            BlackListedPairs badPair = new BlackListedPairs()
            //            {
            //                Base = Pairbase,
            //                Quote = Pairquote,
            //                OwnerId = user.Id
            //            };
            //            dbContext.BlackListedPairs.Add(badPair);
            //            dbContext.SaveChangesAsync();
            //            BotApi.SendMessage(user.TelegramId, string.Format(
            //                CultureTextRequest.GetMessageString("blacklistPairAdded", user.Language), badPair.ToString()));
            //        }
            //        else
            //        {
            //            BotApi.SendMessage(user.TelegramId,string.Format(
            //                CultureTextRequest.GetMessageString("blacklistPairExists", user.Language), blackPairEx.ToString()));
            //        }
            //    }
            //}

        }

    }
}
