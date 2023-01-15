using System.Text.RegularExpressions;

namespace TelegramBot.Commands
{
    public class MonitoringPairsCommand
    {
        public static Regex CreatePair = new(@"/create\s*((?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})\s*(?<price>([0-9.]+)|)|)");
        public static Regex EditPair = new(@"/(edit)\s*((((?<id>[0-9]+)\s*((?<price>[0-9.e]*)|))|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");
        public static Regex DeletePair = new(@"/(delete|remove)\s*((((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");
        public static Regex ShowPair = new(@"/(show)\s*((((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");
        public static Regex TriggerOncePair = new(@"/(triggeronce)\s*((?<id>[0-9]+)\s*|)");
    }
}