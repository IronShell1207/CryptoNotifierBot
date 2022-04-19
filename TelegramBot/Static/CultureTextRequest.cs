using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Constants;

namespace TelegramBot.Static
{
    public class CultureTextRequest
    {
        public static string GetMessageString(string key, string language)
        {
            CultureInfo ci = new CultureInfo(language);
            ResourceManager rm = new ResourceManager(typeof(TelegramBot.Messages));
            return rm.GetString(key, ci);
        }

        public static string GetSettingsMsgString(string key, string language)
        {
            CultureInfo culture = new CultureInfo(language);
            ResourceManager rManager = new ResourceManager(typeof(SettingsMessages));
            return rManager.GetString(key, culture);
        }
    }
}
