using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using TelegramBot.Static;

namespace TelegramBot.Objects
{
    public class CryptoPair : ICloneable
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
        public bool Triggered { get; set; } = false;
        public string? Screenshot { get; set; }
        public string? Note { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public UserConfig? User { get; set; }
        public CryptoPair(){}
        public CryptoPair(int owner)
        {
            OwnerId = owner;
        }

        public bool AreEqual(CryptoPair pair)
        {
            var props = this.GetType().GetProperties().Where(x => x.PropertyType.IsValueType && x.PropertyType == typeof(string));
            foreach (var prop in props)
            {
                var obj1 = prop.GetValue(this);
                var obj2 = prop.GetValue(pair);
                if (!obj1.Equals(obj2))
                    return false;

            }
            return true;
        }

        public override string ToString() => !string.IsNullOrWhiteSpace(PairBase) && !string.IsNullOrWhiteSpace(PairQuote)
            ? $"{PairBase}/{PairQuote}"
            : "";

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string ToStringWithLink()
        {
            var linkExh = CryptoApi.Constants.ExchangesSpotLinks.GetSpotLink(this.ExchangePlatform);
            var link =
            $"<a href='{ExchangesSpotLinks.GetPairLink(this.ExchangePlatform, this.PairBase, this.PairQuote)}'>{this.ToString()}</a>";
            return link;
        }

        private string EnabledDisabled(bool isEn) => isEn ? "Enabled ☑️" : "Disabled ❌";
        public string TaskStatus()
        {
            var enabled = Enabled ? "✅" : "⛔️";
            var rofl = GainOrFall ? ">" : "<";
            var triggered = TriggerOnce ? "💎" : "";
            triggered = Triggered && TriggerOnce ? "🌗" : triggered;
            return $"{enabled} #{Id} {this.ToString()} {rofl}{Price} {triggered}";
        }
        public string TaskStatusWithLink()
        {
            var enabled = Enabled ? "✅" : "⛔️";
            var rofl = GainOrFall ? "&#62;" : "&#60;";
            var triggered = TriggerOnce ? "💎" : "";
            triggered = Triggered && TriggerOnce ? "🌗" : triggered;
            return $"{enabled} #{Id} {this.ToStringWithLink()} {rofl}{Price} {triggered}";
        }

        public string FullTaskInfo(string lang = "en", bool tipsEnable = true)
        {
            var enableSymbol = CultureTextRequest.GetSettingsMsgString("enabled", lang);
            var disableSymbol = CultureTextRequest.GetSettingsMsgString("disabled", lang);
            var link =
                $"<a href='{ExchangesSpotLinks.GetPairLink(this.ExchangePlatform, this.PairBase, this.PairQuote)}'>{this.ToString()}</a>";
            string enable = Enabled ? enableSymbol : disableSymbol;
            string lessOrGreater = GainOrFall ? "&#62;" : "&#60;";
            string lessOrGreaterSymbol = GainOrFall ? "📈" : "📉";
            StringBuilder sb = new StringBuilder(CultureTextRequest.GetSettingsMsgString("taskInfoTitle", lang));
            sb.AppendLine("");
            sb.AppendLine($"Task id: #{this.Id}");
            sb.AppendLine($"{CultureTextRequest.GetSettingsMsgString("taskInfoSymbol", lang)}{link}");
            sb.AppendLine($"{CultureTextRequest.GetSettingsMsgString("taskInfoActiveStatus", lang)}{enable}");
            sb.AppendLine($"{CultureTextRequest.GetSettingsMsgString("taskInfoTriggerPrice", lang)}{lessOrGreater}{this.Price} {lessOrGreaterSymbol}");
            sb.AppendLine($"{CultureTextRequest.GetSettingsMsgString("taskInfoExchangePlatform", lang)}{this.ExchangePlatform}");
            if (TriggerOnce) sb.AppendLine($"Single trigger: {this.EnabledDisabled(this.TriggerOnce)} 💎");
            if (Triggered) sb.AppendLine("Trigger fired 🌗"); 
            if (!string.IsNullOrEmpty(Note)) sb.AppendLine($"Notes: {Note}");
            return sb.ToString();

        }

        public TradingPair ToTradingPair()
        {
            return new TradingPair(PairBase, PairQuote, ExchangePlatform);
        }

    }
}
