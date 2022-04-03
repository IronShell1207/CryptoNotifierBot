using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects.ExchangesPairs
{
    public class GateIOData
    {
        public string currency_pair { get; set; }
        public string last { get; set; }
        public string lowest_ask { get; set; }
        public string highest_bid { get; set; }
        public string change_percentage { get; set; }
        public string base_volume { get; set; }
        public string quote_volume { get; set; }
        public string high_24h { get; set; }
        public string low_24h { get; set; }
        public string etf_net_value { get; set; }
        public string etf_pre_net_value { get; set; }
        public int etf_pre_timestamp { get; set; }
        public string etf_leverage { get; set; }
    }


}
