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
    public class GateioApi
    {
        public static List<CryptoExchangePairInfo> PairsListConverter(List<GateIOData> list)
        {
            var listReturner = new List<CryptoExchangePairInfo>();
            if (list != null)
            {
                foreach (GateIOData pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.currency_pair);
                    if (pairSymbol != null)
                        listReturner.Add(new CryptoExchangePairInfo(pairSymbol, double.Parse(pair.last)));
                    //listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.currency_pair), double.Parse(pair.last)));
                }
            }

            return listReturner;
        }

        public static TradingPair SplitSymbolConverter(string symbol)
        {
            var name = symbol.Split("_").FirstOrDefault();
            var quote = symbol.Split("_").LastOrDefault();
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

        public  static List<GateIOData> GetTickerFullData()
        {
            RestResponse response = RestRequester.GetRequest(new Uri(ExchangesApiLinks.GateIOSpotTicker)).Result;
            if (response?.StatusCode == HttpStatusCode.OK)
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<List<GateIOData>>(new JsonTextReader(new StringReader(response.Content)));
            }

            return new List<GateIOData>()
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
                Exchange = Exchanges.GateIO
            };

        }
    }
}
