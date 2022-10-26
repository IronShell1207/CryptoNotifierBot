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
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CryptoWidgets\";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string FavCursPath => MainFolder + "favcurs.json";

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
        public List<string> SavedPairs { get; set; } = new List<string>();
        public double WidgetOpacity { get; set; } = 0.8;
        public double WidgetFontsOpacity { get; set; } = 0.8;
        public double WidgetScale { get; set; } = 1.0;
    }
}