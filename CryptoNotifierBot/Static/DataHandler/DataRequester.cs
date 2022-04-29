using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.API;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using Microsoft.EntityFrameworkCore;

namespace CryptoApi.Static.DataHandler
{
    public class DataRequester : TheDisposable
    {
        public bool UpdaterLive { get; set; } = true;
        public int DataDownloadedCounter { get; private set; } = 0;
        public List<ITradingApi> apis = new()
        {
            new BitgetApi(),
            new BinanceApi(),
            new GateioApi(),
            new KucoinAPI(),
            new OkxApi()
        };

        public void UpdateAllData()
        {
            var datenow = DateTime.Now;
            StringBuilder sb = new StringBuilder($"[{datenow}] Market data updated: ");
            var guid = Guid.NewGuid();
            foreach (var api in apis)
            {
                api.GetExchangeData(guid);
                sb.Append($"{api.ApiName}: {api.PairsCount} ");
            }
            Console.WriteLine(sb.ToString());
        }

        public void RemoveOldData()
        {
            var date = DateTime.Now - TimeSpan.FromDays(2);
            using (DataBaseContext dbContext = new DataBaseContext())
                if (dbContext.DataSet.Any(x => x.DateTime < date))
                {
                    var rows = dbContext.Database.ExecuteSqlRaw(
                         $"DELETE FROM DataSet Where Id in (SELECT Id FROM DataSet Where date <= \"{date.ToString()}\" ORDER BY Id LIMIT 500)");
                    Console.WriteLine($"[{DateTime.Now}] Rows deleted {rows} for data older {date}");
                }
        }
        public async Task UpdateDataLoop()
        {
            RemoveOldData();
            if (DataDownloadedCounter > 3000)
                using (DataBaseContext dbContext = new DataBaseContext())
                {
                    var rows = dbContext.Database.ExecuteSqlRaw(
                             "DELETE FROM DataSet WHERE Id in (SELECT Id FROM DataSet ORDER BY Id LIMIT 200)");
                    Console.WriteLine($"[{DateTime.Now}] Rows deleted {rows} for data when storage overflow");
                }
            while (UpdaterLive)
            {
                UpdateAllData();
                DataDownloadedCounter++;
                Thread.Sleep(29900);
            }
        }

        public async Task<List<PricedTradingPair>> GetLatestDataByExchangeName(string exchange)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var dSet = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == exchange);
                var pairs = dbContext.TradingPairs.Where(x => x.CryDbSetId == dSet.Id && x.Exchange == dSet.Exchange).ToList();
                return pairs;
            }
        }

        public async Task<List<CryDbSet>> GetLatestDataSets(int minutesOffset = 0)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                
                var latestData = minutesOffset == 0 ? dbContext.DataSet.OrderByDescending(x=>x.Id).FirstOrDefault() :
                    dbContext.DataSet.OrderByDescending(x => x.Id).FirstOrDefault(x => x.DateTime > DateTime.Now.AddMinutes(-minutesOffset));
                var lastdataSets = dbContext.DataSet.OrderByDescending(x => x.Id)
                   .Where(x => x.IdGuid == latestData.IdGuid);
                return lastdataSets?.ToList();
            }
        }

        public async Task<List<CryDbSet>> GetAllDbSets()
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                return dbContext.DataSet.ToList();
            }
        }

        public async Task<PricedTradingPair> GetCurrentPricePairByName(TradingPair pairname, string exchange = "")
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {

                var dbSet = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == exchange);
                //
                //   dbSet = dbContext.DataSet.OrderBy(x => x.Id);
                var pair = dbContext.TradingPairs.FirstOrDefault(x =>
                    x.CryDbSetId == dbSet.Id &&
                    x.Exchange == exchange &&
                    x.Name == pairname.Name &&
                    x.Quote == pairname.Quote);
                return pair ?? null;
            }
        }
        public static void GetExchangesForPair(TradingPair pair)
        {
            List<string> exchanges = new List<string>();
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                //var latestDataId = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault().Id;
                //if (binancePairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                //    exchanges.Add(CryptoApi.Constants.Exchanges.Binance);
                //if (gateioPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                //    exchanges.Add(CryptoApi.Constants.Exchanges.GateIO);
                //if (okxPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                //    exchanges.Add(CryptoApi.Constants.Exchanges.Okx);
                //if (kucoinPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                //    exchanges.Add(CryptoApi.Constants.Exchanges.Kucoin);
                //if (bitgetPairsData.Pairs.Exists(x => x.Symbol.ToString() == pair.ToString()))
                //    exchanges.Add(CryptoApi.Constants.Exchanges.Bitget);
                //return exchanges;
            }

        }
    }
}
