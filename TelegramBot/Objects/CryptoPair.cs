using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using TelegramBot.Static;

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

        public string ToStringWithLink()
        {
            var linkExh = CryptoApi.Constants.ExchangesSpotLinks.GetSpotLink(this.ExchangePlatform);
            var link =
            $"<a href='{linkExh}{ExchangesSpotLinks.GetPairConverted(this.ExchangePlatform, this.PairBase, this.PairQuote)}'>{this.ToString()}</a>";
            return link;
        }

        public string TaskStatus()
        {
            var enabled = Enabled ? "✅" : "⛔️";
            var rofl = GainOrFall ? "&#62;" : "&#60;";
            return $"{enabled} #{Id} {this.ToStringWithLink()} {rofl}{Price}";
        }
        public string TaskStatusWithLink()
        {
            var enabled = Enabled ? "✅" : "⛔️";
            var rofl = GainOrFall ? "&#62;" : "&#60;";
            return $"{enabled} #{Id} {this.ToStringWithLink()} {rofl}{Price}";
        }

        public string FullTaskInfo(string lang = "en")
        {
            var enableSymbol = CultureTextRequest.GetSettingsMsgString("enabled", lang);
            var disableSymbol = CultureTextRequest.GetSettingsMsgString("disabled", lang);
            var linkExh = CryptoApi.Constants.ExchangesSpotLinks.GetSpotLink(this.ExchangePlatform);
            var link =
                $"<a href='{linkExh}{ExchangesSpotLinks.GetPairConverted(this.ExchangePlatform, this.PairBase, this.PairQuote)}'>{this.ToString()}</a>";
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
            if (!string.IsNullOrEmpty(Note)) sb.AppendLine($"Notes: {Note}");
            return sb.ToString();

        }

    }
}
