    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.Static
{
    public class BinanceApi
    {
        public static List<CryptoExchangePairInfo> BinanceExchangePairsListConverter(List<BinancePair> list)
        {
            var listReturner = new List<CryptoExchangePairInfo>();
            foreach (var pair in list)
            {
                listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.symbol), pair.price));
            }

            return listReturner;
        }

        public static CryptoPair SplitSymbolConverter(string symbol)
        {
            var crp = new CryptoPair();
            
            var match = ExchangesRegexCombins.cryptoSymbol.Match(symbol);
            if (match.Success)
            {
                crp.Name = match.Groups["name"].Value;
                crp.Quote = match.Groups["quote"].Value;
            }
            return crp;
        }
        public static SymbolTimedExInfo GetExchangeInfo()
        {
            RestResponse responce = RestRequester.GetRequest(new Uri(ExchangesApiLinks.BinanceClearTicker)).Result;
            JsonSerializer serializer = new JsonSerializer();

            var pairsSerialized = serializer.Deserialize<List<BinancePair>>(new JsonTextReader(new StringReader(responce.Content)));
            var pairs = BinanceExchangePairsListConverter(pairsSerialized);
            return new SymbolTimedExInfo()
            {
                CreationTime = DateTime.Now,
                Pairs = pairs
            };
        }

        public static BinanceSymbolsData GetFullData()
        {
            RestResponse response = RestRequester.GetRequest(new Uri(ExchangesApiLinks.BinanceFullExchangeInfoTicker)).Result;
            JsonSerializer serializer = new JsonSerializer();
            var pairsSetialized =
                serializer.Deserialize<BinanceSymbolsData>(new JsonTextReader(new StringReader(response.Content)));
            return pairsSetialized;
        }
    }
}
