using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using CryptoApi.Static.DataHandler;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.Static
{
    public class KucoinAPI : TheDisposable, ITradingApi
    {
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
                    Exchange = Exchanges.Kucoin
                };
            }
            return null;
        }

        public async Task<List<KucoinData.Ticker>> GetTickerData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = await restRequester.GetRequest(new Uri(ExchangesApiLinks.KucoinSpotTicker));
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var kudata =
                        serializer.Deserialize<KucoinData.Rootobject>(
                            new JsonTextReader(new StringReader(response.Content)));
                    return kudata.data.ticker.ToList();
                }
            }

            return new List<KucoinData.Ticker>()
            {
            };
        }
        public void SavePairsToDb(string exchange, List<PricedTradingPair> pairs)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var dbSet = new CryDbSet(DateTime.Now, exchange);
                dbContext.DataSet.Add(dbSet);
                dbContext.SaveChanges();
                pairs.ForEach(x => x.DbId = dbSet.Id);
                dbContext.TradingPairs.AddRange(pairs);
                dbContext.SaveChanges();
            }
        }
        public async Task GetExchangeData()
        {
            var pairs = PairsListConverter(await GetTickerData());
            SavePairsToDb(Exchanges.Kucoin, pairs);
            //return new SymbolTimedExInfo()
            //{
            //    CreationTime = DateTime.Now,
            //    Pairs = pairs,
            //    Exchange = Exchanges.Kucoin
            //};

        }
    }
}
