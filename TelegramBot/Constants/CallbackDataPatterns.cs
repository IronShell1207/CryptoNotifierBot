using System.Text.RegularExpressions;

namespace TelegramBot.Constants
{
    public class CallbackDataPatterns
    {
        public static string DeletePair = "%delete{0}&{1}";
        public static string EditPair = "%edit{0}&{1}";
        public static Regex DeletePairRegex = new Regex(@"\%delete(?<id>[0-9]*)\&(?<ownerId>[0-9*])");
        public static Regex EditPairRegex = new Regex(@"\%edit(?<id>[0-9]*)\&(?<ownerId>[0-9*])");
    }
}
