using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants
{
    public class RegexCombins
    {
        public static Regex TelegramBotToken = new Regex(@"[0-9]+\:.*");
        public static Regex CryptoPairRegex = new Regex(@"(?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})");
    }
}
