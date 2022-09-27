using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects.ExchangesPairs
{
    public class BinanceSymbolsData
    {
        public string timezone { get; set; }
        public long serverTime { get; set; }
        public Ratelimit[] rateLimits { get; set; }
        public object[] exchangeFilters { get; set; }
        public Symbol[] symbols { get; set; }
    }

    public class Ratelimit
    {
        public string rateLimitType { get; set; }
        public string interval { get; set; }
        public int intervalNum { get; set; }
        public int limit { get; set; }
    }

    public class Symbol
    {
        public string symbol { get; set; }
        public string status { get; set; }
        public string baseAsset { get; set; }
        public int baseAssetPrecision { get; set; }
        public string quoteAsset { get; set; }
        public int quotePrecision { get; set; }
        public int quoteAssetPrecision { get; set; }
        public int baseCommissionPrecision { get; set; }
        public int quoteCommissionPrecision { get; set; }
        public string[] orderTypes { get; set; }
        public bool icebergAllowed { get; set; }
        public bool ocoAllowed { get; set; }
        public bool quoteOrderQtyMarketAllowed { get; set; }
        public bool isSpotTradingAllowed { get; set; }
        public bool isMarginTradingAllowed { get; set; }
        public Filter[] filters { get; set; }
        public string[] permissions { get; set; }
    }

    public class Filter
    {
        public string filterType { get; set; }
        public string minPrice { get; set; }
        public string maxPrice { get; set; }
        public string tickSize { get; set; }
        public string multiplierUp { get; set; }
        public string multiplierDown { get; set; }
        public int avgPriceMins { get; set; }
        public string minQty { get; set; }
        public string maxQty { get; set; }
        public string stepSize { get; set; }
        public string minNotional { get; set; }
        public bool applyToMarket { get; set; }
        public int limit { get; set; }
        public int maxNumOrders { get; set; }
        public int maxNumAlgoOrders { get; set; }
        public string maxPosition { get; set; }
    }
}