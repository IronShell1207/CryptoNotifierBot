using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Update;
using Telegram.Bot;
using TelegramBot.Constants;

namespace TelegramBot.Static
{
    public class ConsoleCommandsHandler
    {
        public static async Task ChangeTokenAsync()
        {
            setbottoken:
            LogWrite("Set telegram bot token: ");
            var reader = Console.ReadLine();
            var token = RegexCombins.TelegramBotToken.Match(reader);
            if (token.Success)
            {
                AppSettingsStatic.Settings.TelegramBotToken = token.Value;
                LogWrite("Bot token saved!");
                JsonHelper.SaveJson(AppSettingsStatic.Settings, AppSettingsStatic.SettsPath);
                LogWrite("You need to restart program to apply changes!");
            }
            else
            {
                ConsoleCommandsHandler.LogWrite("Token invalid. Try again");
                goto setbottoken;
            }
        }

        public static async Task StartBotAsync()
        {
            LogWrite("Starting telegram bot...");
            LogWrite("Connecting to telegram api...");
            await Task.Run(BotApi.StartBotAsync);
            var botInfo = await BotApi.BotClient.GetMeAsync();
            LogWrite($"Bot connected! Bot info: {botInfo}");
        }

        public static async Task SetAdminId()
        {
            setid:
            ConsoleCommandsHandler.LogWrite("Set admin telegram id: ");
            var id = Console.ReadLine();
            try
            {
                await Task.Run(() => BotApi.SendMessage(id, "Bot has been configured! Admin rights granted"));
                AppSettingsStatic.Settings.AdminTelegramId = id;
                LogWrite("Admin set success");
                JsonHelper.SaveJson(AppSettingsStatic.Settings, AppSettingsStatic.SettsPath);
            }
            catch (Exception ex)
            {
                LogWrite($"{ex.Message} Try to start chat with bot or check your blacklist!");
                goto setid;
            }
        }

        public static string DataDir
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Tcryptobot\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public static void LogWrite(string line)
        {
            DateTime dt = DateTime.Now;
            var logFile = DataDir + "log.txt";
            var lineS = $"[{dt.ToString()}] {line}";
            File.AppendAllText(logFile, lineS+"\n");
            Console.WriteLine(lineS);
        }
    }
}
