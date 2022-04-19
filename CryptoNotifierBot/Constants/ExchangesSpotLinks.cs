using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Constants
{
    public class ExchangesSpotLinks
    {
        /// <summary>
        /// Okx spot page, pair pattern base-quote
        /// </summary>
        public const string OkxSpot = "https://www.okx.com/ru/trade-spot/";
        /// <summary>
        /// Binance spot page, pair pattern BASE_QUOTE
        /// </summary>
        public const string BinanceSpot = "https://www.binance.com/ru/trade/";
        /// <summary>
        /// GateIO spot page, pair pattern BASE_QUOTE
        /// </summary>
        public const string GateIoSpot = "https://www.gate.io/tradepro/";
        /// <summary>
        /// GateIO spot page, pair pattern BASE-QUOTE
        /// </summary>
        public const string KucoinSpot = "https://www.kucoin.com/ru/trade/";

        public static string GetSpotLink(string exchange)
        {
            if (exchange == null)
                throw new ArgumentNullException(nameof(exchange));
            if (exchange == Exchanges.Binance) return BinanceSpot;
            else if (exchange == Exchanges.Okx) return OkxSpot;
            else if (exchange == Exchanges.GateIO) return GateIoSpot;
            else if (exchange == Exchanges.Kucoin) return KucoinSpot;
            return null;
        }
        public static string GetPairConverted(string exchange, string Base, string Quote)
        {
            if (exchange == null)
                throw new ArgumentNullException(nameof(exchange));
            if    (exchange == Exchanges.Binance)  return $"{Base}_{Quote}";
            else if (exchange == Exchanges.Okx)    return $"{Base}-{Quote}";
            else if (exchange == Exchanges.GateIO) return $"{Base}-{Quote}";
            else if (exchange == Exchanges.Kucoin) return $"{Base}_{Quote}";
            return null;
        }
    }
}
