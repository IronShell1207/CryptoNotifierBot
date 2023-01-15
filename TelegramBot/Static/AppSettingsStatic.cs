using TelegramBot.Helpers;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class AppSettingsStatic
    {
        private static AppSettings _settings;

        public static string SettsPath
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Tcryptobot\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path + "config.json";
            }
        }

        public static AppSettings Settings
        {
            get
            {
                if (_settings != null) return _settings;
                _settings = File.Exists(SettsPath) ? JsonHelper.LoadSettings(SettsPath) : new AppSettings();
                return _settings;
            }
        }
    }
}