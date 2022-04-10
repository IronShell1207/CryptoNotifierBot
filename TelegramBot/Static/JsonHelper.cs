using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class JsonHelper
    {
        public static AppSettings LoadSettings(string path)
        {
            using (StreamReader jsReader = new StreamReader(path))
            {
                JsonReader json = new JsonTextReader(jsReader);
                JsonSerializer jsonSerializer = new JsonSerializer();
                var list = jsonSerializer.Deserialize<AppSettings>(json);
                return list;
            }
        }
        public static void SaveJson(object listSet, string path)
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                JsonWriter jsonWriter = new JsonTextWriter(sw);
                JsonSerializer jsnS = new JsonSerializer();
                jsnS.Formatting = Formatting.Indented;
                jsnS.Serialize(jsonWriter, listSet);
            }
        }
    }
}

