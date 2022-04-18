using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;

namespace TelegramBot.Objects
{
    public class BreakoutPair
    {
        public TradingPair Symbol { get; set; }
        public double? oldPrice { get; set; }
        public double? newPrice { get; set; }
        public double Time { get; set; }
        public string Exchange { get; set; }

    }
}
