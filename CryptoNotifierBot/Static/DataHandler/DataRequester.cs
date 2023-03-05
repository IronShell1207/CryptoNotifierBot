using CryptoApi.API;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoApi.Static.DataHandler
{
    public class DataLoader
    {
        public void GetBinanceData(Guid guid)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            using var api = new ExchangeApi(Exchanges.Binance);
            var result = api.GetExchangeData<List<BinancePair>>(guid).Result;

            timer.Stop();
            Console.WriteLine($"{api.ApiName}: {api.PairsCount} in {timer.Elapsed} secs.");
        }

        public void GetOkxData(Guid guid)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            using var api = new ExchangeApi(Exchanges.Okx);
            var result = api.GetExchangeData<OkxData>(guid).Result;

            timer.Stop();
            Console.WriteLine($"{api.ApiName}: {api.PairsCount} in {timer.Elapsed} secs.");
        }

        public void GetGateIOData(Guid guid)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            using var api = new ExchangeApi(Exchanges.GateIO);
            var result = api.GetExchangeData<List<GateIOTicker>>(guid).Result;

            timer.Stop();
            Console.WriteLine($"{api.ApiName}: {api.PairsCount} in {timer.Elapsed} secs.");
        }

        public void GetKucoinData(Guid guid)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                using var api = new ExchangeApi(Exchanges.Kucoin);
                var result = api.GetExchangeData<KucoinData>(guid).Result;

                timer.Stop();
                Console.WriteLine($"{api.ApiName}: {api.PairsCount} in {timer.Elapsed} secs.");
            }
            catch (Exception ex) { }
        }

        public void GetKucoinFullData(Guid guid)
        {
            using (var api = new ExchangeApi(Exchanges.Kucoin))
            {
                var result = api.GetTickerData<KucoinData>().Result?.data?.ticker;
                if (result != null)
                {
                    using (DataBaseContext dbContext = new DataBaseContext())
                    {
                        var rows = dbContext.KucoinPairs.Where(x => x.Id > -1);
                        Diff.LogWrite($"Rows deleted {rows.Count()} in KucoinPairs", ConsoleColor.DarkYellow);
                        dbContext.RemoveRange(rows);
                        //var rows = dbContext.Database.ExecuteSqlRaw($"DELETE FROM KucoinPairs");

                        for (var index = 0; index < result.Length; index++)
                        {
                            var tickData = result[index];
                            dbContext.KucoinPairs.Add(new KuTickerDB(tickData));
                        }

                        dbContext.SaveChanges();
                    }
                    Diff.LogWrite($"{api.ApiName} data saved: {result.Length}");
                }
                else
                {
                    Diff.LogWrite($"{api.ApiName} data load fail.", ConsoleColor.DarkRed);
                }
            }
        }

        public void GetOkxFullData(Guid guid)
        {
            using (var api = new ExchangeApi(Exchanges.Okx))
            {
                var result = api.GetTickerData<OkxData>().Result?.data;
                if (result != null)
                    using (DataBaseContext dbContext = new DataBaseContext())
                    {
                        var rows = dbContext.OkxPairs.Where(x => x.Id > -1);
                        Diff.LogWrite($"Rows deleted {rows.Count()} in OkxPairs", ConsoleColor.DarkYellow);
                        dbContext.RemoveRange(rows);

                        //var rows = dbContext.Database.ExecuteSqlRaw($"DELETE FROM OkxPairs");

                        for (var index = 0; index < result.Length; index++)
                        {
                            var tickData = result[index];
                            dbContext.OkxPairs.Add(new OkxTickerDB(tickData));
                        }
                        dbContext.SaveChanges();
                    }
                Diff.LogWrite($"{api.ApiName} data saved: {result?.Length}", ConsoleColor.DarkGreen);
            }
        }
    }

    public class DataRequester : TheDisposable
    {
        public bool UpdaterLive { get; set; } = true;
        public int DataDownloadedCounter { get; private set; } = 0;
        public bool DataAvailable { get; set; } = false;
        public TimeSpan UpdateDelay { get; set; } = TimeSpan.FromSeconds(10);

        private int Try = 30;

        public void UpdateParallelly()
        {
            Guid guid = Guid.NewGuid();
            var methods = typeof(DataLoader).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var parfams = new List<object> { guid };
            ;
            Parallel.ForEach(methods, (method) =>
            {
                if (method.ReturnType.Name == "Void")
                    method.Invoke(new DataLoader(), parfams.ToArray());
            });
        }

        public void UpdateByExchange(string exchange)
        {
            Guid guid = Guid.NewGuid();
            var methods = typeof(DataLoader).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var parfams = new List<object> { guid };

            foreach (var method in methods)
                if (method.ReturnType.Name == "Void" && method.Name.ToLower().Contains(exchange.ToLower()))
                    method.Invoke(new DataLoader(), parfams.ToArray());
        }

        public void UpdateKucoinData()
        {
            Guid guid = Guid.NewGuid();
            var methods = typeof(DataLoader).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var parfams = new List<object> { guid };

            foreach (var method in methods)
                if (method.ReturnType.Name == "Void" && method.Name == "GetKucoinFullData")
                    method.Invoke(new DataLoader(), parfams.ToArray());
        }

        public void UpdateOkcData()
        {
            Guid guid = Guid.NewGuid();
            var methods = typeof(DataLoader).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var parfams = new List<object> { guid };

            foreach (var method in methods)
                if (method.ReturnType.Name == "Void" && method.Name == "GetOkxFullData")
                    method.Invoke(new DataLoader(), parfams.ToArray());
        }

        public void RemoveOldData(TimeSpan time)
        {
            var date = DateTime.Now - time;
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                if (dbContext.DataSet.Any(x => x.DateTime < date))
                {
                    var rows = dbContext.Database.ExecuteSqlRaw(
                         $"DELETE FROM DataSet Where Id in (SELECT Id FROM DataSet Where DateTime <= \"{date.ToString()}\" ORDER BY Id LIMIT 800)");
                    Diff.LogWrite($"Rows deleted {rows} for data older {date}", ConsoleColor.Red);
                }

                if (dbContext.DataSet.Any(x => x.IdGuid == new Guid()))
                {
                    var rows = dbContext.Database.ExecuteSqlRaw(
                        $"DELETE FROM DataSet Where Id in (SELECT Id FROM DataSet Where IdGuid = \"{default(Guid)}\" ORDER BY Id LIMIT 500)");
                    Diff.LogWrite($"Rows deleted {rows} for data older {date}", ConsoleColor.Red);
                }
            }
        }

        public async Task UpdateDataLoop()
        {
            while (UpdaterLive)
            {
                UpdateParallelly();
                RemoveOldData(TimeSpan.FromHours(60));
                DataAvailable = true;
                await Task.Delay(45000);
            }
        }

        public async Task UpdateDataLoop(string exchange)
        {
            while (UpdaterLive)
            {
                UpdateByExchange(exchange);
                RemoveOldData(TimeSpan.FromMinutes(2));
                DataAvailable = true;
                await Task.Delay(UpdateDelay);
            }
        }

        public async Task UpdateKucoinLoop()
        {
            while (UpdaterLive)
            {
                UpdateKucoinData();
                RemoveOldData(TimeSpan.FromMinutes(2));
                DataAvailable = true;
                await Task.Delay(UpdateDelay);
            }
        }

        public async Task UpdateOkxLoop()
        {
            while (UpdaterLive)
            {
                UpdateOkcData();
                RemoveOldData(TimeSpan.FromMinutes(2));
                DataAvailable = true;
                await Task.Delay(UpdateDelay);
            }
        }

        public async Task<List<PricedTradingPair>> GetLatestDataByExchangeName(string exchange, int limit = 9999)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var dSet = dbContext.DataSet.Include(x => x.pairs).OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == exchange);
                return dSet?.pairs?.Take(limit)?.ToList();
                var pairs = dbContext.TradingPairs.Where(x => x.CryDbSetId == dSet.Id && x.Exchange == dSet.Exchange).Take(limit).ToList();
                return pairs;
            }
        }

        public async Task<KuTickerDB> GetKucoinData(string name, string quote)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                string symbol = $"{name.ToUpper()}{Exchanges.ExApiSeparator(Exchanges.Kucoin)}{quote.ToUpper()}";
                var dbData =
                    dbContext.KucoinPairs.OrderBy(x => x.Id).FirstOrDefault(x => x.symbol == symbol);
                return dbData;
            }
        }

        public async Task<OkxTickerDB> GetOkxData(string name, string quote)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                try
                {
                    string symbol = $"{name.ToUpper()}{Exchanges.ExApiSeparator(Exchanges.Okx)}{quote.ToUpper()}";
                    var dbData =
                        dbContext.OkxPairs.OrderByDescending(x => x.Id);
                    var par = dbData.FirstOrDefault(x => x.instId == symbol);
                    return par;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task<List<CryDbSet>> GetLatestDataSets(int minutesOffset = 0)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var latestData = minutesOffset == 0 ? dbContext.DataSet.OrderByDescending(x => x.Id).FirstOrDefault() :
                    dbContext.DataSet.OrderByDescending(x => x.Id).FirstOrDefault(x =>
                        x.DateTime > DateTime.Now.AddMinutes(-minutesOffset) &&
                        DateTime.Now.AddMinutes(-minutesOffset + 1) > x.DateTime);
                if (latestData == null) return null;
                var lastdataSets = dbContext.DataSet.Include(c => c.pairs).OrderByDescending(x => x.Id)
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
                {
                    var exchanges = await GetExchangesForPair(pairname);
                    if (!exchanges.Any()) return null;
                    pairname.Exchange = exchanges.FirstOrDefault();
                }
                var dbSet = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == pairname.Exchange);
                var pair = dbContext.TradingPairs.FirstOrDefault(x =>
                    x.CryDbSetId == dbSet.Id &&
                    x.Exchange == pairname.Exchange &&
                    x.Name == pairname.Name &&
                    x.Quote == pairname.Quote);
                return pair;
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