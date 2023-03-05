using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CryptoApi.Objects.ExchangesPairs
{
    public class MexcData
    {
        public int code { get; set; }
        public MexcPair[] data { get; set; }
    }

    public class MexcPair : TheTradingPair
    {
        [JsonProperty("symbol")]
        public override string Symbol { get; set; }
        public string volume { get; set; }
        public string amount { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string bid { get; set; }
        [JsonProperty("ask")]
        public string ask { get; set; }
        public string open { get; set; }

        [JsonProperty("last")]
        public override string Price { get; set; }
        public long time { get; set; }
        public string change_rate { get; set; }
    }

}
