using System.Text.RegularExpressions;

namespace TelegramBot.Constants
{
    public class CallbackDataPatterns
    {
        public static string DeletePair = "%delete{0}&{1}";
        public static Regex DeletePairRegex = new Regex(@"\%delete(?<id>)\&(?<ownerId>)");
    }
}
