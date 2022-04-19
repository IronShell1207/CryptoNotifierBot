using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class CryptoPair
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string PairBase { get; set; }
        public bool Enabled { get; set; }
        public string PairQuote { get; set; }
        public string ExchangePlatform { get; set; }
        public bool GainOrFall { get; set; }
        public double Price { get; set; }
        public bool TriggerOnce { get; set; } = false;
        public string? Screenshot { get; set; }
        public string? Note { get; set; }

        public CryptoPair(){}
        public CryptoPair(int owner)
        {
            OwnerId = owner;
        }
        public override string ToString()
        {
            return $"{PairBase}/{PairQuote}";
        }

        public string TaskStatus()
        {
            var enabled = Enabled ? "✅" : "⛔️";
            var rofl = GainOrFall ? ">" : "<";
            return $"{enabled} #{Id} {PairBase}/{PairQuote} {rofl}{Price}";
        }

    }
}
