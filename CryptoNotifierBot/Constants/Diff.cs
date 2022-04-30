using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Constants
{
    public class Diff
    {
        public static List<string> AllowedQuotes = new List<string>()
        {
            "BTC", "USDT"
        };

        public static string MainPath
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Tcryptobot\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string LogPath => MainPath + "log.txt";

        public static void LogWrite(string line)
        {
            DateTime dt = DateTime.Now;
            var logFile = LogPath;
            var lineS = $"[{dt.ToString()}] {line}";
            File.AppendAllText(logFile, lineS + "\n");
            Console.WriteLine(lineS);
        }
    }
}
