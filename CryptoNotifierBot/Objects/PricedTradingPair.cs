using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public class PricedTradingPair : TradingPair
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public CryDbSet? CryDbSet { get; set; }
        public int CryDbSetId { get; set; }
        public PricedTradingPair(){}
        public PricedTradingPair(TradingPair pair, double price)
        {
            this.Name = pair.Name;
            this.Quote = pair.Quote;
            this.Exchange = pair.Exchange;
            this.Price= price;  
        }

        public PricedTradingPair(string Base, string Quote, string Exchange, double Price)
        {
            this.Name = Base;
            this.Quote = Quote;
            this.Price = Price;
            this.Exchange = Exchange;
        }
    }
}
