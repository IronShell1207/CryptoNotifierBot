using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using CryptoApi.Constants;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.Static
{
    public class ExchangeDataKeeper
    {
        public static List<CryptoPair> ExchangePairs { get; set; } = new List<CryptoPair>();
        public static List<BinanceSymbolsData> BinancePairs { get; set; } = new List<BinanceSymbolsData>();
        public static List<SymbolTimedExInfo> HistoryExchangePairs { get; set; } = new List<SymbolTimedExInfo>();

        public  static void DataUpdater()
        {
            HistoryExchangePairs.Add(BinanceApi.GetExchangeInfo());
            Thread.Sleep(30000);
            //var restOptions = new RestClientOptions()
            //{
            //    ThrowOnAnyError = true,
            //    Timeout = 500,
            //    UserAgent = WebHeaders.UserAgent,

            //};
            //var client = new RestClient(restOptions);
            //var rq
            //var pInfo = client.ExecuteGetAsync(new RestRequest(new Uri(ExchangesApiLinks.BinanceClearTicker)));
            //pInfo.Result.

        }
    }
}
