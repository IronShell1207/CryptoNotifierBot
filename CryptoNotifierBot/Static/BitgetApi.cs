using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public class BitgetApi : TheDisposable, ITradingApi
    {
        public List<PricedTradingPair> ExchangePairsConverter(List<BitgetTicker> list)
        {
            var listReturner = new List<PricedTradingPair>();
            if (list != null)
            {
                foreach (BitgetTicker pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.symbol);
                    if (pairSymbol != null)
                        listReturner.Add(new PricedTradingPair(pairSymbol, double.Parse(pair.buyOne, new CultureInfo("en"))));
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
                crp.Exchange = Exchanges.Bitget;
                if (Diff.AllowedQuotes.Contains(crp.Quote))
                    return crp;
            }
            return null;
        }
        public async Task GetExchangeData()
        {
            var pairs = ExchangePairsConverter(await GetTickerData());
            SavePairsToDb(Exchanges.Bitget, pairs);
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
        public async Task<List<BitgetTicker>> GetTickerData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = await restRequester.GetRequest(new Uri(ExchangesApiLinks.BitgetSpotTicker));
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var data = serializer.Deserialize<BitgetData>(
                        new JsonTextReader(new StringReader(response.Content)));
                    if (data.msg == "success")
                        return data.data.ToList();
                }
            }

            return new List<BitgetTicker>()
            {

            };
        }
    }
}
