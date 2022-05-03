﻿using System;
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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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
            StringBuilder sb = new StringBuilder($"Market data updated: ");
            var guid = Guid.NewGuid();
            foreach (var api in apis)
            {
                api.GetExchangeData(guid);
                sb.Append($"{api.ApiName}: {api.PairsCount} ");
            }
            Diff.LogWrite(sb.ToString());
        }

        public void RemoveOldData()
        {
            var date = DateTime.Now - TimeSpan.FromHours(60);
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                if (dbContext.DataSet.Any(x => x.DateTime < date))
                {
                    var rows = dbContext.Database.ExecuteSqlRaw(
                         $"DELETE FROM DataSet Where Id in (SELECT Id FROM DataSet Where DateTime <= \"{date.ToString()}\" ORDER BY Id LIMIT 800)");
                    Diff.LogWrite($"Rows deleted {rows} for data older {date}");
                }

                if (dbContext.DataSet.Any(x => x.IdGuid == new Guid()))
                {
                    var rows = dbContext.Database.ExecuteSqlRaw(
                        $"DELETE FROM DataSet Where Id in (SELECT Id FROM DataSet Where IdGuid = \"{default(Guid)}\" ORDER BY Id LIMIT 500)");
                    Diff.LogWrite($"Rows deleted {rows} for data older {date}");
                }

            }
        }
        public async Task UpdateDataLoop()
        {
            if (DataDownloadedCounter > 3000)
                using (DataBaseContext dbContext = new DataBaseContext())
                {
                    var rows = dbContext.Database.ExecuteSqlRaw(
                             "DELETE FROM DataSet WHERE Id in (SELECT Id FROM DataSet ORDER BY Id LIMIT 200)");
                    Diff.LogWrite($"Rows deleted {rows} for data when storage overflow");
                }
            while (UpdaterLive)
            {
                UpdateAllData();
                RemoveOldData();
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
                var latestData = minutesOffset == 0 ? dbContext.DataSet.OrderByDescending(x => x.Id).FirstOrDefault() :
                    dbContext.DataSet.OrderByDescending(x => x.Id).FirstOrDefault(x => 
                        x.DateTime > DateTime.Now.AddMinutes(-minutesOffset) &&
                        DateTime.Now.AddMinutes(-minutesOffset+1) > x.DateTime);
                if (latestData == null) return null;
                var lastdataSets = dbContext.DataSet.Include(c=>c.pairs).OrderByDescending(x => x.Id)
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

        public async Task<PricedTradingPair> GetCurrentPricePairByName(string baseName, string quoteName,
            string exchange = "") =>
            await GetCurrentPricePairByName(new TradingPair(baseName, quoteName, exchange));

        public async Task<PricedTradingPair> GetCurrentPricePairByName(TradingPair pairname)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                if (string.IsNullOrEmpty(pairname.Exchange))
                    pairname.Exchange = GetExchangesForPair(pairname).Result.FirstOrDefault();
                var dbSet = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == pairname.Exchange);
                var pair = dbContext.TradingPairs.FirstOrDefault(x =>
                    x.CryDbSetId == dbSet.Id &&
                    x.Exchange == pairname.Exchange &&
                    x.Name == pairname.Name &&
                    x.Quote == pairname.Quote);
                return pair ?? null;
            }
        }
        public async Task<List<String>> GetExchangesForPair(TradingPair pair)
        {
            List<string> exchanges = new List<string>();
            var dataSets = await GetLatestDataSets();
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                foreach (var dbSet in dataSets)
                {
                    if (dbContext.TradingPairs.OrderByDescending(x => x.Id).FirstOrDefault(
                                  z => z.CryDbSet == dbSet &&
                                       z.Name == pair.Name &&
                                       z.Quote == pair.Quote) != null)
                        exchanges.Add(dbSet.Exchange);
                }
                return exchanges;
            }

        }
    }
}
