using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class PairsManager
    {
        public static List<CryptoPair> TempObjects { get; set; } = new List<CryptoPair>();
    }
}
