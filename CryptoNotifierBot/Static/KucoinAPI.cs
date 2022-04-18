using System;
using System.Collections.Generic;
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
    public class KucoinAPI
    {
        public static List<CryptoExchangePairInfo>PairsListConverter(List<KucoinData.Ticker> list)
        {
            var listReturner = new List<CryptoExchangePairInfo>();
            if (list != null)
            {
                foreach (KucoinData.Ticker pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.symbol);
                    if (pairSymbol != null)
                        listReturner.Add(new CryptoExchangePairInfo(pairSymbol, double.Parse(pair.last)));
                    // listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.symbol), double.Parse(pair.last)));
                }
            }
            return listReturner;
        }

        public static TradingPair SplitSymbolConverter(string symbol)
        {
            var name = symbol.Split("-").FirstOrDefault();
            var quote = symbol.Split("-").LastOrDefault();
            if (Diff.AllowedQuotes.Contains(quote))
            {
                return new TradingPair()
                {
                    Name = name,
                    Quote = quote
                };
            }
            return null;
        }

        public static List<KucoinData.Ticker> GetTickerFullData()
        {
            RestResponse response = RestRequester.GetRequest(new Uri(ExchangesApiLinks.KucoinSpotTicker)).Result;
            if (response?.StatusCode == HttpStatusCode.OK)
            {
                JsonSerializer serializer = new JsonSerializer();
                var kudata = serializer.Deserialize<KucoinData.Rootobject>(new JsonTextReader(new StringReader(response.Content)));
                return kudata.data.ticker.ToList();
            }

            return new List<KucoinData.Ticker>()
            {
            };
        }
        public static SymbolTimedExInfo GetExchangeData()
        
        {
            var pairs = PairsListConverter(GetTickerFullData());
            return new SymbolTimedExInfo()
            {
                CreationTime = DateTime.Now,
                Pairs = pairs,
                Exchange = Exchanges.Kucoin
            };

        }
    }
}
