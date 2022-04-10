using System;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBot;
using TelegramBot.Constants;
using TelegramBot.Objects;
using TelegramBot.Static;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Crypto notifications bot loading...");

            if (string.IsNullOrWhiteSpace(AppSettingsStatic.Settings.TelegramBotToken))
            {
                setbottoken:
                Console.Write("Set telegram bot token: ");
                var token = RegexCombins.TelegramBotToken.Match(Console.ReadLine());
                if (token.Success)
                {
                    AppSettingsStatic.Settings.TelegramBotToken = token.Value;
                    Console.WriteLine("Bot token saved!");
                    JsonHelper.SaveJson(AppSettingsStatic.Settings, "config.json");
                }
                else
                {
                    Console.WriteLine("Token invalid. Try again");
                    goto setbottoken;
                }
            }

            Console.WriteLine("Starting telegram bot...");
            Task.Run(BotApi.StartBotAsync);
            while (!BotApi.IsBotStarted)
            {
                Console.WriteLine("Connecting to telegram api...");
                Thread.Sleep(300);
            }

            Console.WriteLine($"Bot connected! Bot info: {BotApi.BotClient.GetMeAsync().Result}");
            if (string.IsNullOrWhiteSpace(AppSettingsStatic.Settings.AdminTelegramId))
            {
                SetAdminId().Wait();
            }
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Bot has been started!");
            
           // BotApi.SendMessage(1328993812, "test");
            Task.Run(() => NotifyLoops.MainLoop());
           while (true)
            {
                Console.ReadLine();
            }
        }

        private static async Task SetAdminId()
        {
            setid:
            Console.Write("Set admin telegram id: ");
            var id = Console.ReadLine();
            try
            {
                await Task.Run(() => BotApi.SendMessage(id, "Bot has been configured! Admin rights granted"));
                AppSettingsStatic.Settings.AdminTelegramId = id;
                Console.WriteLine("Admin set success");
                JsonHelper.SaveJson(AppSettingsStatic.Settings, "config.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} Try to start chat with bot or check your blacklist!");
                goto setid;
            }
        }

        private static async Task SetDbContext()
        {
        }

    }
}
