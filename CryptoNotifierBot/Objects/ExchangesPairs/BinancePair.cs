using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects.ExchangesPairs
{
    public class BinanceData : ITradingApiData
    {
        public BinancePair[] data { get; set; }
    }
    public class BinancePair : TheTradingPair
    {
        public string symbol { get; set; }

        public override string Symbol => symbol;

        public double price { get; set; }

        public override string Price => price.ToString();
    }
}