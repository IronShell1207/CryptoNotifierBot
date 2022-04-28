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
    public class GateioApi : TheDisposable, ITradingApi
    {
        public List<PricedTradingPair> ExchangePairsConverter(List<GateIOData> list)
        {
            var listReturner = new List<PricedTradingPair>();
            if (list != null)
            {
                foreach (GateIOData pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.currency_pair);
                    if (pairSymbol != null)
                        listReturner.Add(new PricedTradingPair(pairSymbol, double.Parse(pair.last, new CultureInfo("en"))));
                    //listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.currency_pair), double.Parse(pair.last)));
                }
            }

            return listReturner;
        }

        public TradingPair SplitSymbolConverter(string symbol)
        {
            var name = symbol.Split("_").FirstOrDefault();
            var quote = symbol.Split("_").LastOrDefault();
            if (Diff.AllowedQuotes.Contains(quote))
            {
                return new TradingPair()
                {
                    Name = name,
                    Quote = quote,
                    Exchange = Exchanges.GateIO
                };
            }
            return null;
        }

        public async Task<List<GateIOData>> GetTickerFullData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = await restRequester.GetRequest(new Uri(ExchangesApiLinks.GateIOSpotTicker));
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<List<GateIOData>>(
                        new JsonTextReader(new StringReader(response.Content)));
                }
                else if (response?.StatusCode == null)
                    return null;
            }

            return new List<GateIOData>()
            {

            };
        }
        public void SavePairsToDb(string exchange, List<PricedTradingPair> pairs)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                if (pairs.Any())
                {
                    var dbSet = new CryDbSet(DateTime.Now, exchange);
                    dbContext.DataSet.Add(dbSet);
                    dbContext.SaveChanges();
                    pairs.ForEach(x => x.DbId = dbSet.Id);
                    dbContext.TradingPairs.AddRange(pairs);
                    dbContext.SaveChanges();
                }
            }
        }
        public async Task GetExchangeData()
        {
            var pairs = ExchangePairsConverter(await GetTickerFullData());
            SavePairsToDb(Exchanges.GateIO, pairs);
        }
    }
}
