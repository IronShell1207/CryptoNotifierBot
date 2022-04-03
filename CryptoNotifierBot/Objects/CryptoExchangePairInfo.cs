using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public class CryptoExchangePairInfo 
    {
        public CryptoPair Symbol { get; set; }
        public double? Price { get; set; }

        public CryptoExchangePairInfo(CryptoPair symbol, double? price)
        {
            Symbol = symbol;
            Price = price;
        }
    }
}
