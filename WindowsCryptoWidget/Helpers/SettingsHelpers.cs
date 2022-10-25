using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsCryptoWidget.Helpers
{
    internal class SettingsHelpers
    {
        public static string MainFolder
        {
            get
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ACryptoWidgets\";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string FavCursPath = MainFolder + "favcurs.json";

        private static Settings _settingsConfig;

        public static Settings SettingsConfig
        {
            get
            {
                if (_settingsConfig != null)
                    return _settingsConfig;

                return _settingsConfig = JsonHelper.LoadJson<Settings>(FavCursPath) ?? new Settings();
            }
        }
    }

    public class Settings
    {
        public List<string> FavPairs { get; set; } = new List<string>();
        public double WOpacity { get; set; } = 0.8;
        public double WSize { get; set; } = 0.5;
    }
}