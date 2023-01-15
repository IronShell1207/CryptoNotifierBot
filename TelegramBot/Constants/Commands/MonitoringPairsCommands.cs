using System.Text.RegularExpressions;

namespace TelegramBot.Constants.Commands
{
    public sealed class MonitoringPairsCommands
    {
        public static Regex AddMonPairsCommandRegex =
            new(
                @"/addmon\s*(?<base>[a-zA-Z0-9]{2,9})");

        public static Regex DelMonPairsCommandRegex =
            new(
                @"/delmon\s*(?<base>[a-zA-Z0-9]{2,9})");
    }
}