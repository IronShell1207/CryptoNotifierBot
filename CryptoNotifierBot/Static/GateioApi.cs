using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            foreach (GateIOData pair in list)
            {
                listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.currency_pair), double.Parse(pair.last)));
            }

            return listReturner;
        }

        public static CryptoPair SplitSymbolConverter(string symbol)
        {
            var name = symbol.Split("_").FirstOrDefault();
            var quote = symbol.Split("_").LastOrDefault();
            return new CryptoPair()
            {
                Name = name,
                Quote = quote
            };
        }

        public static List<GateIOData> GetTickerFullData()
        {
            RestResponse response = RestRequester.GetRequest(new Uri(ExchangesApiLinks.GateIOSpotTicker)).Result;
            if (response.IsSuccessful)
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<List<GateIOData>>(new JsonTextReader(new StringReader(response.Content)));
            }

            return new List<GateIOData>()
            {
            };
        }
        public static SymbolTimedExInfo GetExchangeInfo()
        {
            var pairs = PairsListConverter(GetTickerFullData());
            return new SymbolTimedExInfo()
            {
                CreationTime = DateTime.Now,
                Pairs = pairs
            };

        }
    }
}
