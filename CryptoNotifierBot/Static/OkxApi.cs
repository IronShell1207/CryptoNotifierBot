using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.Static
{
    public class OkxApi : TheDisposable, ITradingApi
    {
        public List<PricedTradingPair> PairsListConverter(List<OkxPairsInfo> list)
        {
            var listReturner = new List<PricedTradingPair>();
            if (list != null)
            {
                foreach (OkxPairsInfo pair in list)
                {
                    var pairSymbol = SplitSymbolConverter(pair.instId);
                    if (pairSymbol != null)
                        listReturner.Add(new PricedTradingPair(pairSymbol, double.Parse(pair.last, new CultureInfo("en"))));
                    // listReturner.Add(new CryptoExchangePairInfo(SplitSymbolConverter(pair.instId), double.Parse(pair.last)));
                }
            }

            return listReturner;
        }

        public TradingPair SplitSymbolConverter(string symbol)
        {
            var name = symbol.Split("-").FirstOrDefault();
            var quote = symbol.Split("-").LastOrDefault();
            if (Diff.AllowedQuotes.Contains(quote))
            {
                return new TradingPair()
                {
                    Name = name,
                    Quote = quote,
                    Exchange = Exchanges.Okx
                };
            }
            return null;
        }

        public async Task<List<OkxPairsInfo>> GetTickerData()
        {
            using (var restRequester = new RestRequester())
            {
                RestResponse response = await restRequester.GetRequest(new Uri(ExchangesApiLinks.OkxSpotTicker));
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var data = serializer.Deserialize<OkxData>(new JsonTextReader(new StringReader(response.Content)));
                    return data.data.ToList();
                }
            }

            return new List<OkxPairsInfo>();
        }

        public async Task GetExchangeData()
        {
            var pairs = PairsListConverter(await GetTickerData());
            //return new SymbolTimedExInfo()
            //{
            //    CreationTime = DateTime.Now,
            //    Pairs = pairs,
            //    Exchange = Exchanges.Okx
            //};

        }

    }
}
