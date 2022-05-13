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
        public async Task<List<PricedTradingPair>> GetLatest100Pairs()
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
                    return dbContext.CryptoPairs.Include(x=>x.User).ToList();
                else return dbContext.CryptoPairs.Include(x => x.User).Where(
                    x=>x.OwnerId == userid).ToList();
            }
        }
        public async Task<List<UserConfig>> Users()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                return dbContext.Users.Include(x => x.pairs).ToList();
            }
        }

        public async Task<(UserConfig, BreakoutSub)> userconfigGET(int id)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var user = dbContext.Users.
                    Include(x => x.pairs).
                    FirstOrDefault(x=>x.Id == id);
                var usersets = dbContext.BreakoutSubs.OrderBy(x => x.Id).FirstOrDefault(x => x.UserId == user.Id);
                return (user, usersets);

            }
        }

        public async Task<CryptoPair> GetUserCPSetts(int taskid)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                return dbContext.CryptoPairs.Include(x => x.User).FirstOrDefault(x => x.Id == taskid);
            }
        }

        public  async Task SaveTaskSettings(CryptoPair pairExt)
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
            }
            
        }

        public async Task SaveUserSettings(UserConfig userConfig, BreakoutSub sub)
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
            }
        }
    }
}