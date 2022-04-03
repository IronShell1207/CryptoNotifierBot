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
    public class OkxApi
    {
        public static List<CryptoExchangePairInfo> PairsListConverter(List<OkxPairsInfo> list)
        {
            var listReturner = new List<CryptoExchangePairInfo>();
            foreach (OkxPairsInfo pair in list)
            {
                listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.instId), double.Parse(pair.last)));
            }

            return listReturner;
        }

        public static CryptoPair SplitSymbolConverter(string symbol)
        {
            var name = symbol.Split("-").FirstOrDefault();
            var quote = symbol.Split("-").LastOrDefault();
            return new CryptoPair()
            {
                Name = name,
                Quote = quote
            };
        }

        public static SymbolTimedExInfo GetExchangeInfo()
        {
            RestResponse response = RestRequester.GetRequest(new Uri(ExchangesApiLinks.OkxSpotTicker)).Result;
            
            if (response.IsSuccessful)
            {
                JsonSerializer serializer = new JsonSerializer();
                var pairsSerialized =
                    serializer.Deserialize<OkxData>(new JsonTextReader(new StringReader(response.Content)));
                var pairs = PairsListConverter(pairsSerialized.data.ToList());
                return new SymbolTimedExInfo()
                {
                    CreationTime = DateTime.Now,
                    Pairs = pairs
                };
            }
            else return new SymbolTimedExInfo() { };

        }

    }
}
