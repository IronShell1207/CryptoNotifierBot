using System;
using System.Collections.Generic;
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
            new BitgetApi(), new BinanceApi(), new GateioApi(), new KucoinAPI(), new OkxApi()
        };
        public void UpdateAllData()
        {
            StringBuilder sb = new StringBuilder($"[{DateTime.Now.ToString()}] Market data updated: ");
            foreach (var api in apis)
            {
                api.GetExchangeData();
                sb.Append($"{api.ApiName}: {api.PairsCount} ");
            }
            Console.WriteLine(sb.ToString());
        }
        public async Task UpdateDataLoop()
        {
            if (DataDownloadedCounter >3000)
                using (DataBaseContext dbContext = new DataBaseContext())
                {
                    var firstRows =
                        dbContext.DataSet.FromSqlRaw(
                            "DELETE FROM DataSet WHERE Id in (SELECT Id FROM DataSet ORDER BY Id LIMIT 4)");
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
               var dSet = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x=>x.Exchange == exchange);
               var pairs = dbContext.TradingPairs.Where(x => x.CryDbSetId == dSet.Id && x.Exchange == dSet.Exchange).ToList();
               return pairs;
            }
        }

        public async Task<List<CryDbSet>> GetAllDbSets()
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                return dbContext.DataSet.ToList();
            }
        }

        public async Task<PricedTradingPair> GetCurrentPricePairByName(string exchange, TradingPair pairname)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var dbSet = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == exchange);
                var pair = dbContext.TradingPairs.FirstOrDefault(x =>
                    x.CryDbSetId == dbSet.Id && 
                    x.Exchange == exchange && 
                    x.Name == pairname.Name &&
                    x.Quote == pairname.Quote);
                return pair ?? null;
            }
        }
    }
}
