using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects.ExchangesPairs
{
    public class BitgetData
    {
        public string code { get; set; }
        public BitgetTicker[] data { get; set; }
        public string msg { get; set; }
        public string requestTime { get; set; }
    }
    public class BitgetTicker : TheTradingPair
    {
        public string baseVol { get; set; }
        public string buyOne { get; set; }
        public string close { get; set; }
        public string high24h { get; set; }
        public string low24h { get; set; }
        public string quoteVol { get; set; }
        public string sellOne { get; set; }
        public string symbol { get; set; }
        public string ts { get; set; }
        public string usdtVol { get; set; }

        public override string Symbol => symbol;
        public override string Price => sellOne;
    }
}
