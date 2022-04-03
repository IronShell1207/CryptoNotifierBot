using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class CryptoPair
    {
        public int Id { get; set; }
        public int UserOwnerId { get; set; }
        public string PairBase { get; set; }
        public string PairQuote { get; set; }
        public string ExchangePlatform { get; set; }
        public bool GainOrFall { get; set; }
        public double Price { get; set; }
        public bool TriggerOnce { get; set; }
        public string Screenshot { get; set; }
        public string Note { get; set; }
    }
}
