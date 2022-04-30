using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using CryptoApi.Static;
using CryptoApi.Static.DataHandler;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.API
{
    public class KucoinAPI : TheDisposable, ITradingApi
    {
        public string ApiName => Exchanges.Kucoin;
        public int PairsCount { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public List<PricedTradingPair> PairsListConverter(List<KucoinData.Ticker> list)
        {
            var listReturner = new List<PricedTradingPair>();
            if (list != null)
            {
                foreach (KucoinData.Ticker pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.symbol);
                    if (pairSymbol != null)
                        listReturner.Add(new PricedTradingPair(pairSymbol, double.Parse(pair.last, new CultureInfo("en"))));
                    // listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.symbol), double.Parse(pair.last)));
                }
            }
            return listReturner;
        }

        public TradingPair SplitSymbolConverter(string symbol)
        {
            var name = symbol.Split("-").FirstOrDefault();
            var quote = symbol.Split("-").LastOrDefault();
            if (Diff.AllowedQuotes.Contains(quote))
            {
                return new TradingPair()
                {
                    Name = name,
                    Quote = quote,
                    Exchange = ApiName
                };
            }
            return null;
        }

        public async Task<List<KucoinData.Ticker>> GetTickerData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = await restRequester.GetRequest(new Uri(ExchangesApiLinks.KucoinSpotTicker), ApiName);
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var kudata =
                        serializer.Deserialize<KucoinData.Rootobject>(
                            new JsonTextReader(new StringReader(response.Content)));
                    PairsCount = kudata.data.ticker.ToList().Count;
                    LastUpdate = DateTime.Now;
                    return kudata.data.ticker.ToList();
                }
                else if (response?.StatusCode == null)
                    return null;
                else
                {
                    Diff.LogWrite(
                        $"{ApiName} api request failed. Status code: {response?.StatusCode}, {response?.ErrorMessage}");
                    Thread.Sleep(4000);
                    return await GetTickerData();
                }
            }
            return new List<KucoinData.Ticker>()
            {
            };
        }
        public void SavePairsToDb(string exchange, List<PricedTradingPair> pairs, Guid guid)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                if (pairs.Any())
                {
                    var dbSet = new CryDbSet(DateTime.Now, exchange, guid);
                    dbSet.pairs = pairs;
                    dbContext.DataSet.Add(dbSet);
                    dbContext.SaveChanges();
                }
            }
        }
        public async Task GetExchangeData(Guid guid = default(Guid))
        {
            var pairs = PairsListConverter(await GetTickerData());
            SavePairsToDb(Exchanges.Kucoin, pairs, guid);
        }
    }
}
