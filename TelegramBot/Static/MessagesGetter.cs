using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Static
{
    public class MessagesGetter
    {
        public static string GetGlobalString(string key, string language)
        {
            CultureInfo ci = new CultureInfo(language);
            ResourceManager rm = new ResourceManager(typeof(TelegramBot.Messages));
            return rm.GetString(key, ci);
        }
       // public static string GetSettingsMsgString(string key)
    }
}
