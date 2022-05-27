using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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

            PairsCount = listReturner.Count;
            return listReturner;
        }

        private IEnumerable<TheTradingPair> GetFromProps(PropertyInfo[] props, object crdata)
        {
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(Int64))
                    continue;
                var value = prop?.GetValue(crdata);
                if (value is IEnumerable<TheTradingPair>)
                    return (IEnumerable<TheTradingPair>)value;

                if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                {
                    var h = GetFromProps(prop.PropertyType.GetProperties(), crdata);
                    if (h.Any())
                        return h;
                }
            }
            return new List<TheTradingPair>();
        }

        public List<PricedTradingPair> PairsListConverter(object crdata)
        {
            var type = crdata?.GetType();
            if (type.IsGenericType)
            {
                var obj = (IEnumerable<TheTradingPair>)crdata;
                return Convert(obj);
            }
            var props = type.GetProperties();
            return Convert(GetFromProps(props, crdata));
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
        public async Task<bool> GetExchangeData<T>(Guid guid = default(Guid))
        {
            var apiData = await GetTickerData<T>();
            if (apiData == null) return false;
            var pairs = PairsListConverter(apiData);
            if (pairs != null) SavePairsToDb(ApiName, pairs, guid);
            return true;
        }

        public ExchangeApi(string apiName)
        {
            ApiName = apiName;
        }
    }
}
