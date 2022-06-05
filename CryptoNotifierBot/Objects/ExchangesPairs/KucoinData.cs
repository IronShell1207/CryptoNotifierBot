using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects.ExchangesPairs
{

    public class KucoinData 
    {
        public string code { get; set; }
        public KuData data { get; set; }
    }

    public class KuData
    {
        public long time { get; set; }
        public KuTicker[] ticker { get; set; }
    }

    public class KuTicker : TheTradingPair
    {   
        public string symbol { get; set; }
        public string symbolName { get; set; }
        public string buy { get; set; }
        public string sell { get; set; }
        public string changeRate { get; set; }
        public string changePrice { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string vol { get; set; }
        public string volValue { get; set; }
        public string last { get; set; }
        public string averagePrice { get; set; }
        public string takerFeeRate { get; set; }
        public string makerFeeRate { get; set; }
        public string takerCoefficient { get; set; }
        public string makerCoefficient { get; set; }
        public override string Symbol => symbol;    
        public override string Price => last;
    }

}
