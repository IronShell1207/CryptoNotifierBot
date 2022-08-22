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
            await Task.Run(BotApi.StartBot);
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
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Tcryptobot\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }


        public async static void LogWrite(string line)
        {

            DateTime dt = DateTime.Now;
            var logFile = DataDir + "log.txt";
            var lineS = $"[{dt.ToString()}] {line}";
            start:
            try
            {
                File.AppendAllText(logFile, lineS + "\n");

            }
            catch (IOException ex)
            {
                if (ex.HResult == -2147024864)
                {
                    Thread.Sleep(300);
                    goto start;
                }

            }
            Console.WriteLine(lineS);


        }
    }
    internal class ConsoleSpinner
    {
        private int _currentAnimationFrame;

        public ConsoleSpinner()
        {
            SpinnerAnimationFrames = new[]
            {
                "[#=======]",
                "[=#======]",
                "[==#=====]",
                "[===#====]",
                "[====#===]",
                "[=====#==]",
                "[======#=]",
                "[=======#]",
                "[======#=]",
                "[=====#==]",
                "[====#===]",
                "[===#====]",
                "[==#=====]",
                "[=#======]",
                "[#=======]",
            };
        }

        public string[] SpinnerAnimationFrames { get; set; }

        public void UpdateProgress()
        {
            // Store the current position of the cursor
            var originalX = Console.CursorLeft;
            var originalY = Console.CursorTop;

            // Write the next frame (character) in the spinner animation
            Console.Write(SpinnerAnimationFrames[_currentAnimationFrame]);

            // Keep looping around all the animation frames
            _currentAnimationFrame++;
            if (_currentAnimationFrame == SpinnerAnimationFrames.Length)
            {
                _currentAnimationFrame = 0;
            }

            // Restore cursor to original position
            Console.SetCursorPosition(originalX, originalY);
        }
    }
}
