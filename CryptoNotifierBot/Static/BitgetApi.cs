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
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.Static
{
    public class BitgetApi : TheDisposable, ITradingApi
    {
        public List<CryptoExchangePairInfo> ExchangePairsConverter(List<BitgetTicker> list)
        {
            var listReturner = new List<CryptoExchangePairInfo>();
            if (list != null)
            {
                foreach (BitgetTicker pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.symbol);
                    if (pairSymbol != null)
                        listReturner.Add(new CryptoExchangePairInfo(pairSymbol, double.Parse(pair.buyOne, new CultureInfo("en"))));
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
                if (Diff.AllowedQuotes.Contains(crp.Quote))
                    return crp;
            }
            return null;
        }
        public SymbolTimedExInfo GetExchangeData()
        {
            var pairs = ExchangePairsConverter(GetTickerFullData());
            return new SymbolTimedExInfo()
            {
                CreationTime = DateTime.Now,
                Pairs = pairs,
                Exchange = Exchanges.Bitget
            };
        }
        public List<BitgetTicker> GetTickerFullData()
        {
            RestResponse response = RestRequester.GetRequest(new Uri(ExchangesApiLinks.BitgetSpotTicker)).Result;
            if (response?.StatusCode == HttpStatusCode.OK)
            {
                JsonSerializer serializer = new JsonSerializer();
                var data = serializer.Deserialize<BitgetData>(new JsonTextReader(new StringReader(response.Content)));
                if (data.msg == "success")
                    return data.data.ToList();
            }

            return new List<BitgetTicker>()
            {

            };
        }
    }
}
