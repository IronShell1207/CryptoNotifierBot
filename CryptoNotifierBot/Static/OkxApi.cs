using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
            if (list != null)
            {
                foreach (OkxPairsInfo pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.instId);
                    if (pairSymbol != null)
                        listReturner.Add(new CryptoExchangePairInfo(pairSymbol, double.Parse(pair.last, new CultureInfo("en"))));
                    // listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.instId), double.Parse(pair.last)));
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

        public static OkxData GetTickerFullData()
        {

            RestResponse response = RestRequester.GetRequest(new Uri(ExchangesApiLinks.OkxSpotTicker)).Result;
            if (response?.StatusCode == HttpStatusCode.OK)
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<OkxData>(new JsonTextReader(new StringReader(response.Content)));
            }

            return new OkxData()
            {
                data = new OkxPairsInfo[0]
            };
        }

        public static SymbolTimedExInfo GetExchangeData()
        {
            var pairs = PairsListConverter(GetTickerFullData()?.data?.ToList());
            return new SymbolTimedExInfo()
            {
                CreationTime = DateTime.Now,
                Pairs = pairs,
                Exchange = Exchanges.Okx
            };

        }

    }
}
