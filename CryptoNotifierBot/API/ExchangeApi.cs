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
    public class ExchangeApi : TheDisposable
    {

        public string ApiName { get; }
        public int PairsCount { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public List<PricedTradingPair> Convert(IEnumerable<TheTradingPair> pairs)// where T : IEnumerable<TheTradingPair>
        {
            var listReturner = new List<PricedTradingPair>();
            foreach (var pair in pairs)
            {
                var symbol = SplitSymbolBySplitConverter(pair.Symbol);
                if (symbol != null)
                {
                    var pricedSymbol = new PricedTradingPair(symbol, double.Parse(pair.Price, new CultureInfo("en")));
                  if (pricedSymbol != null) listReturner.Add(pricedSymbol);
                }
            }

            return listReturner;
        }
        public List<PricedTradingPair> PairsListConverter<T>(T crdata)
        {
            if (crdata != null)
            {
                if (crdata is KucoinData)
                    return Convert((crdata as KucoinData).data.ticker.ToList());
                else if (crdata is OkxData)
                    return Convert((crdata as OkxData).data.ToList());
                else if (crdata is List<GateIOData>)
                    return Convert((crdata as List<GateIOData>));
                else if (crdata is List<BinancePair>)
                    return Convert((crdata as List<BinancePair>));
                else if (crdata is BitgetData)
                    return Convert((crdata as BitgetData).data.ToList());
            }

            return null;
        }

        public TradingPair SplitSymbolBySplitConverter(string symbol)
        {
            var splitter = Exchanges.ExApiSeparator(ApiName);
            if (splitter == null) return SplitSymbolConverter(symbol);
            var name = symbol.Split(splitter).FirstOrDefault();
            var quote = symbol.Split(splitter).LastOrDefault();
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

        public async Task<T> GetTickerData<T>()
        {
            using (var restRequester = new RestRequester())
            {
                var apiLink = ExchangesApiLinks.GetApiLink(ApiName);
                RestResponse response = await restRequester.GetRequest(apiLink, ApiName);
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var data = serializer.Deserialize<T>(
                        new JsonTextReader(new StringReader(response.Content)));
                    //PairsCount = data.Count;
                    LastUpdate = DateTime.Now;
                    return data;
                }
                else if (response?.StatusCode == null)
                    return default(T);
                else
                {
                    Diff.LogWrite(
                        $"{ApiName} api request failed. Status code: {response?.StatusCode}, {response?.ErrorMessage}");
                    Thread.Sleep(4000);
                    return await GetTickerData<T>();
                }
            }
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
        public async Task GetExchangeData<T>(Guid guid = default(Guid))
        {
            var pairs = PairsListConverter(await GetTickerData<T>());
            SavePairsToDb(ApiName, pairs, guid);
        }

        public ExchangeApi(string apiName)
        {
            ApiName = apiName;
        }
    }
}
