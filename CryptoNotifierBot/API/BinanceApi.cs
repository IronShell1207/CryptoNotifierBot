using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    public class BinanceApi : TheDisposable, ITradingApi
    {
        public string ApiName => Exchanges.Binance;
        public int PairsCount { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public List<PricedTradingPair> ExchangePairsConverter(List<BinancePair> list)
        {
            var listReturner = new List<PricedTradingPair>();
            if (list != null)
            {
                foreach (BinancePair pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.symbol);
                    if (pairSymbol != null)
                        listReturner.Add(new PricedTradingPair(pairSymbol, pair.price));    
                }
            }

            return listReturner;
        }

        public TradingPair SplitSymbolConverter(string symbol)
        {
            var crp = new TradingPair();
            var match = ExchangesRegexCombins.cryptoSymbol.Match(symbol);
            if (match.Success)
            {
                crp.Name = match.Groups["name"].Value;
                crp.Quote = match.Groups["quote"].Value;
                crp.Exchange = ApiName;
                if (Diff.AllowedQuotes.Contains(crp.Quote))
                    return crp;
            }
            return null;
        }
         
        public async Task<List<BinancePair>> GetTickerData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = await restRequester.GetRequest(new Uri(ExchangesApiLinks.BinanceClearTicker), ApiName);
                JsonSerializer serializer = new JsonSerializer();
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    var pairsSerialized =
                        serializer.Deserialize<List<BinancePair>>(
                            new JsonTextReader(new StringReader(response.Content)));
                    PairsCount = pairsSerialized.Count;
                    LastUpdate = DateTime.Now;
                    return pairsSerialized;
                }
                else if (response?.StatusCode == null)
                    return null;
                else
                {
                    Diff.LogWrite(
                        $"Binance api request failed. Status code: {response?.StatusCode}, {response?.ErrorMessage}");
                    Thread.Sleep(4000);
                    return await GetTickerData();
                }
            }
        }

        public async Task GetExchangeData(Guid guid = default(Guid))
        {
            var pairs = ExchangePairsConverter(await GetTickerData());
            SavePairsToDb(Exchanges.Binance, pairs, guid);
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

        public BinanceSymbolsData GetFullData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = restRequester
                    .GetRequest(new Uri(ExchangesApiLinks.BinanceFullExchangeInfoTicker), ApiName).Result;
                JsonSerializer serializer = new JsonSerializer();
                var pairsSetialized =
                    serializer.Deserialize<BinanceSymbolsData>(new JsonTextReader(new StringReader(response.Content)));
                return pairsSetialized;
            }
            return null;
        }


    }
}
