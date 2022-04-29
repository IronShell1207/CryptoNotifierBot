﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public class TradingPair
    {
        public string Name { get; set; }
        public string Quote { get; set; }
        public string Exchange { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Quote))
                return "";
            return $"{Name}/{Quote}";
        }
        public TradingPair(){}
        public TradingPair(string name, string quote)
        {
            this.Name = name;
            this.Quote = quote;
        }
        public TradingPair(string name, string quote, string exchange)
        {
            Exchange = exchange;
            Name = name;
            Quote = quote;
        }
    }
}
