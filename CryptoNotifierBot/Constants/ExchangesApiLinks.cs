using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Constants
{
    public class ExchangesApiLinks
    {
        public static string BinanceClearTicker = "https://www.binance.com/api/v3/ticker/price";
        public static string BinancePairTicker = "https://www.binance.com/api/v3/ticker/price?symbol={0}";
        public static string BinanceFullExchangeInfoTicker = "https://www.binance.com/api/v1/exchangeInfo";
        public static string OkxSpotTicker = "https://www.okx.com/api/v5/market/tickers?instType=SPOT";
        public static string GateIOSpotTicker = "https://api.gateio.ws/api/v4/spot/tickers";
        public static string KucoinSpotTicker = "https://api.kucoin.com/api/v1/market/allTickers";
    }

}
