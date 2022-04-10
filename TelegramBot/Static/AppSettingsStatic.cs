using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class AppSettingsStatic
    {
        private static AppSettings _settings;
        public static AppSettings Settings
        {
            get
            {
                if (_settings != null) return _settings;
                _settings = File.Exists("config.json") ? JsonHelper.LoadSettings("config.json") : new AppSettings();
                return _settings;
            }
        }
    }
}
