using CryptoApi;
using CryptoApi.Objects;
using CryptoApi.Static.DataHandler;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using TelegramBot;
using TelegramBot.Objects;
using TelegramBot.Static;

namespace CryptoBotWebPortal.Data

{
    public class WeatherForecastService
    {
        /*public async Task<List<PricedTradingPair>> GetLatest100Pairs()
        {
            using (var dbcontext = new DataRequester())
            {
                return await dbcontext.GetLatestDataByExchangeName("Binance", 1000);
            }
        }

        public async Task<List<CryptoPair>> UsersTasks(int userid = 0)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                if (userid == 0)
                    return dbContext.CryptoPairs.Include(x => x.User).ToList();
                else return dbContext.Users.Include(x => x.pairs).OrderBy(x => x.Id).
                    FirstOrDefault(x => x.Id == userid).pairs;
            }
        }

        public async Task<List<UserConfig>> Users()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                return dbContext.Users.Include(x => x.pairs).ToList();
            }
        }

        public async Task<(UserConfig, BreakoutSub, List<MonObj>)> userconfigGET(int id)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var user = dbContext.Users.
                    Include(x => x.pairs).
                    FirstOrDefault(x => x.Id == id);
                var usersets = dbContext.BreakoutSubs.Include(x => x.BlackListedPairsList).OrderBy(x => x.Id).FirstOrDefault(x => x.UserId == user.Id);
                var monTasks = dbContext.MonPairs.Where(x => x.OwnerId == user.Id)?.ToList();
                return (user, usersets, monTasks);
            }
        }

        public async Task<bool> RemoveMonPair(MonObj pair)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pairMon = dbContext.MonPairs.First(x => x.Id == pair.Id && pair.OwnerId == x.Id && x.PairBase == pair.PairBase);
                dbContext.MonPairs.Remove(pairMon);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<CryptoPair> GetUserCPSetts(int taskid)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                return dbContext.CryptoPairs.Include(x => x.User).FirstOrDefault(x => x.Id == taskid);
            }
        }

        public async Task<(bool, string)> SaveTaskSettings(CryptoPair pairExt)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pair = dbContext.CryptoPairs.OrderBy(x => x.Id).FirstOrDefault(x => x.Id == pairExt.Id);
                pair.Price = pairExt.Price;
                pair.PairBase = pairExt.PairBase;
                pair.PairQuote = pairExt.PairQuote;
                pair.ExchangePlatform = pairExt.ExchangePlatform;
                pair.Enabled = pairExt.Enabled;
                pair.GainOrFall = pairExt.GainOrFall;
                pair.TriggerOnce = pairExt.TriggerOnce;
                dbContext.SaveChangesAsync();
                var checkpair = dbContext.CryptoPairs.OrderBy(x => x.Id).FirstOrDefault(x => x.Id == pairExt.Id);
                if (checkpair.AreEqual(pair))
                    return (true, "Saved successfuly");
                else return (false, "Failed to save, no changes are applyed");
            }
        }

        public async Task<(bool, string)> RemovePairFromBL(BreakoutSub user, BlackListedPairs pair)
        {
            try
            {
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var userbreak = dbContext.BreakoutSubs.Include(x => x.BlackListedPairsList).OrderBy(x => x.Id).First(x => x.Id == user.Id);
                    if (userbreak != null)
                    {
                        //var pair = user.BlackListedPairsList.First(x => x == pair);
                        userbreak.BlackListedPairsList.RemoveAll(x => x.Id == pair.Id && x.OwnerId == pair.OwnerId);
                        await dbContext.SaveChangesAsync();
                        return (true, "Успешно удалено");
                    }
                }

                return (false, "Ошибка");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> SaveUserSettings(UserConfig userConfig, BreakoutSub sub)
        {
            try
            {
                using (AppDbContext dbContext = new AppDbContext())
                {
                    var user = dbContext.Users.OrderBy(x => x.Id).First(x => x.Id == userConfig.Id);

                    user.Language = userConfig.Language;
                    user.MorningReport = userConfig.MorningReport;
                    user.NightModeEnable = userConfig.NightModeEnable;
                    user.NightModeEndsTime = userConfig.NightModeEndsTime;
                    user.NightModeStartTime = userConfig.NightModeStartTime;
                    user.NoticationsInterval = userConfig.NoticationsInterval;
                    user.MonInterval = userConfig.MonInterval;
                    user.TimezoneChange = userConfig.TimezoneChange;
                    user.TriggerOneTasksByDefault = userConfig.TriggerOneTasksByDefault;
                    user.DisplayTaskEditButtonsInNotifications = userConfig.DisplayTaskEditButtonsInNotifications;
                    user.RemoveLatestNotifyBeforeNew = userConfig.RemoveLatestNotifyBeforeNew;
                    user.NoticationsInterval = userConfig.NoticationsInterval;
                    if (sub != null)
                    {
                        var breaksub = dbContext.BreakoutSubs.OrderBy(x => x.Id).First(x => x.Id == sub.Id);
                        if (breaksub != null)
                        {
                            breaksub.S120MinUpdates = sub.S120MinUpdates;
                            breaksub.S2MinUpdates = sub.S2MinUpdates;
                            breaksub.S5MinUpdates = sub.S5MinUpdates;
                            breaksub.S15MinUpdates = sub.S15MinUpdates;
                            breaksub.S30MinUpdates = sub.S30MinUpdates;
                            breaksub.S45MinUpdates = sub.S45MinUpdates;
                            breaksub.S60MinUpdates = sub.S60MinUpdates;
                            breaksub.S240MinUpdates = sub.S240MinUpdates;
                            breaksub.S480MinUpdates = sub.S480MinUpdates;
                            breaksub.S960MinUpdates = sub.S960MinUpdates;
                            breaksub.S1920MinUpdates = sub.S1920MinUpdates;
                            breaksub.BinanceSub = sub.BinanceSub;
                            breaksub.BitgetSub = sub.BitgetSub;
                            breaksub.OkxSub = sub.OkxSub;
                            breaksub.GateioSub = sub.GateioSub;
                            breaksub.KucoinSub = sub.KucoinSub;
                            breaksub.Subscribed = sub.Subscribed;
                            breaksub.BlackListEnable = sub.BlackListEnable;
                            breaksub.WhitelistInsteadBlack = sub.WhitelistInsteadBlack;
                        }
                    }

                    dbContext.SaveChangesAsync();
                    var checkuser = dbContext.Users.OrderBy(x => x.Id).First(x => x.Id == userConfig.Id);
                    if (checkuser.AreEqual(user))
                        return (true, "Saved successfuly");
                    else return (false, "Failed to save, no changes are applyed");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }*/
    }
}