using Newtonsoft.Json;
using TelegramBot.Objects;

namespace TelegramBot.Helpers
{
    public class JsonHelper
    {
        public static T LoadSettings<T>(string path)
        {
            using (StreamReader jsReader = new StreamReader(path))
            {
                JsonReader json = new JsonTextReader(jsReader);
                JsonSerializer jsonSerializer = new JsonSerializer();
                var list = jsonSerializer.Deserialize<T>(json);
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