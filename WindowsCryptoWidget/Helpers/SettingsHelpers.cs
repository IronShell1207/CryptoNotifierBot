using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private static Settings _SettingsH;

        public static Settings SettingsH
        {
            get
            {
                if (_SettingsH != null)
                    return _SettingsH;
                if (File.Exists(FavCursPath))
                {
                    _SettingsH = JsonHelper.LoadRCS<Settings>(FavCursPath);
                    return _SettingsH;
                }
                _SettingsH = new Settings { FavPairs = new List<string> { }, WSize = 0.8, WOpacity = 0.5 };
                return _SettingsH;
            }
            set
            {
                _SettingsH = value;
            }
        }
    }

    public class Settings
    {
        public List<string> FavPairs { get; set; }
        public double WOpacity { get; set; }
        public double WSize { get; set; }
    }
}