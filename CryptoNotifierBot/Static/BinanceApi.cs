using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.Static
{
    public class BinanceApi : TheDisposable, ITradingApi
    {

        public List<CryptoExchangePairInfo> ExchangePairsConverter(List<BinancePair> list)
        {
            var listReturner = new List<CryptoExchangePairInfo>();
            //var listbi = list as List<BinancePair>;
            if (list != null)
            {
                foreach (BinancePair pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.symbol);
                    if (pairSymbol != null)
                        listReturner.Add(new CryptoExchangePairInfo(pairSymbol, pair.price));
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
            using (var restRequester = new RestRequester())
            {
                RestResponse response = restRequester.GetRequest(new Uri(ExchangesApiLinks.BinanceClearTicker)).Result;
                JsonSerializer serializer = new JsonSerializer();
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    var pairsSerialized =
                        serializer.Deserialize<List<BinancePair>>(
                            new JsonTextReader(new StringReader(response.Content)));
                    var pairs = ExchangePairsConverter(pairsSerialized);
                    return new SymbolTimedExInfo()
                    {
                        CreationTime = DateTime.Now,
                        Pairs = pairs,
                        Exchange = Exchanges.Binance
                    };
                }

                Console.WriteLine(
                    $"[{DateTime.Now.ToString()}] Binance api request failed. Status code: {response?.StatusCode}, {response?.ErrorMessage}");
                Thread.Sleep(4000);
                return GetExchangeData();

            }
        }

        public BinanceSymbolsData GetFullData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = restRequester
                    .GetRequest(new Uri(ExchangesApiLinks.BinanceFullExchangeInfoTicker)).Result;
                JsonSerializer serializer = new JsonSerializer();
                var pairsSetialized =
                    serializer.Deserialize<BinanceSymbolsData>(new JsonTextReader(new StringReader(response.Content)));
                return pairsSetialized;
            }
            return null;
        }


    }
}
