using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Static.DataHandler;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoApi.API
{
    public class CoinMarketCapTop : TheDisposable
    {
        private string TOKEN = "5ea2ded4-3929-46ed-866f-84ca15192d40";
        private string TopApiUrl = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest";
        private string TopPairsUrl = "https://pro-api.coinmarketcap.com/v2/cryptocurrency/market-pairs/latest";

        public async Task<CoinMarketCapData.Datum[]> Get(int limit= 250)
        {
            var request = new RestSharp.RestRequest(TopApiUrl, Method.Get);
            request.AddHeader("X-CMC_PRO_API_KEY", TOKEN);
            request.AddParameter("limit", limit);
            request.Timeout = 10000;
            request.AddHeader("UserAgent", WebHeaders.UserAgent);
            var result = await new RestClient().ExecuteAsync(request);
            if (result.StatusCode != HttpStatusCode.OK) return null;
            var data = new JsonSerializer().Deserialize<CoinMarketCapData.Rootobject>(new JsonTextReader(new StringReader(result.Content)));
            return data.data;

        }

        public async Task<List<TradingPair>> GetCmcTopPairs(int limit = 250)
        {
            var data = await Get(limit);
            List<TradingPair> convertedPairs = new List<TradingPair>();
            pairsrequest:
            DataRequester dreq = new DataRequester();
            var pairsAvailable = await dreq.GetLatestDataSets();
            if (pairsAvailable == null)
            {
                dreq.UpdateAllData();
                Task.Run(() => Thread.Sleep(2000));
                goto pairsrequest;
            }
            foreach (var pairCmc in data)
            {
               
            }

            return convertedPairs;
        }
    }
}
