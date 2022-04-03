using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects.ExchangesPairs
{
    public class OkxData
    {
        public string code { get; set; }
        public string msg { get; set; }
        public OkxPairsInfo[] data { get; set; }
    }

    public class OkxPairsInfo
    {
        public string instType { get; set; }
        public string instId { get; set; }
        public string last { get; set; }
        public string lastSz { get; set; }
        public string askPx { get; set; }
        public string askSz { get; set; }
        public string bidPx { get; set; }
        public string bidSz { get; set; }
        public string open24h { get; set; }
        public string high24h { get; set; }
        public string low24h { get; set; }
        public string volCcy24h { get; set; }
        public string vol24h { get; set; }
        public string ts { get; set; }
        public string sodUtc0 { get; set; }
        public string sodUtc8 { get; set; }
    }

}
