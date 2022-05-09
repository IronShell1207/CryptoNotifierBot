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

        public async Task<UserConfig> userconfigGET(int id)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                return dbContext.Users.Include(x => x.pairs).FirstOrDefault(x=>x.Id == id);
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
                dbContext.SaveChangesAsync();
            }
             
        }
    }
}