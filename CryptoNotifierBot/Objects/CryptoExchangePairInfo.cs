using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public class CryptoExchangePairInfo 
    {
        public TradingPair Symbol { get; set; }
        public double? Price { get; set; }
       

        public CryptoExchangePairInfo(TradingPair symbol, double? price)
        {
            Symbol = symbol;
            Price = price;
       
        }
    }
}
