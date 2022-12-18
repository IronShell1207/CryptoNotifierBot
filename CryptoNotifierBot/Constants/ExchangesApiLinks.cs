using System;

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
        public static string BitgetSpotTicker = "https://capi.bitgetapi.com/api/spot/v1/market/tickers";
        private static string CoinExSpotTicker = "https://api.coinex.com/v1/market/ticker";

        public static Uri GetApiLink(string exchange)
        {
            switch (exchange)
            {
                case Exchanges.Binance: return new Uri(BinanceClearTicker);
                case Exchanges.Bitget: return new Uri(BitgetSpotTicker);
                case Exchanges.Kucoin: return new Uri(KucoinSpotTicker);
                case Exchanges.GateIO: return new Uri(GateIOSpotTicker);
                case Exchanges.Okx: return new Uri(OkxSpotTicker);
                default: return null;
            }
        }
    }
}