using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
    public class OkxApi : TheDisposable, ITradingApi
    {
        public string ApiName => Exchanges.Okx;
        public int PairsCount { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public List<PricedTradingPair> PairsListConverter(List<OkxPairsInfo> list)
        {
            var listReturner = new List<PricedTradingPair>();
            if (list != null)
            {
                foreach (OkxPairsInfo pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.instId);
                    if (pairSymbol != null)
                        listReturner.Add(new PricedTradingPair(pairSymbol, double.Parse(pair.last, new CultureInfo("en"))));
                    // listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.instId), double.Parse(pair.last)));
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

        public async Task<List<OkxPairsInfo>> GetTickerData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = await restRequester.GetRequest(new Uri(ExchangesApiLinks.OkxSpotTicker));
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var data = serializer.Deserialize<OkxData>(new JsonTextReader(new StringReader(response.Content)));
                    PairsCount = data.data.ToList().Count;
                    LastUpdate = DateTime.Now;
                    return data.data.ToList();
                }
                else if (response?.StatusCode == null)
                    return null;
            }

            return new List<OkxPairsInfo>();
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
            SavePairsToDb(Exchanges.Okx, pairs, guid);
            //return new SymbolTimedExInfo()
            //{
            //    CreationTime = DateTime.Now,
            //    Pairs = pairs,
            //    Exchange = Exchanges.Okx
            //};

        }

    }
}
