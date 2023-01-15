using System.Globalization;
using System.Resources;
using TelegramBot.Constants;

namespace TelegramBot.Helpers
{
    public class CultureTextRequest
    {
        public static string GetMessageString(string key, string language)
        {
            CultureInfo ci = new CultureInfo(language);
            ResourceManager rm = new ResourceManager(typeof(Messages));
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