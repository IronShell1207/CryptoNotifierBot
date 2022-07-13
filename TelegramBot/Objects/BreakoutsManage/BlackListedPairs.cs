using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;

namespace TelegramBot.Objects
{
    public class BlackListedPairs
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public BreakoutSub? Sub { get; set; }
        public string Base { get; set; }
        public string Quote { get; set; }
        public override string ToString() => $"{Base}/{Quote}";
        
        public BlackListedPairs(){}

        public BlackListedPairs(TradingPair pair)
        {
            Base = pair.Name;
            Quote = pair.Quote;
        }

        public BlackListedPairs(string basef, string quote)
        {
            Base = basef;
            Quote = quote;
        }
        public TradingPair ToTradingPair()
        {
            return new TradingPair(Base, Quote, "");
        }

    }
}
