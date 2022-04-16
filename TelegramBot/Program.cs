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
                ConsoleCommandsHandler.ChangeTokenAsync();
            }
            ConsoleCommandsHandler.StartBotAsync();
            if (string.IsNullOrWhiteSpace(AppSettingsStatic.Settings.AdminTelegramId))
            {
                ConsoleCommandsHandler.SetAdminId().Wait();
            }
            Task.Run(() => NotifyLoops.MainLoop());
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Bot has been started!");
            while (true)
            {
                var reader = Console.ReadLine();
                if (reader.ToString() == "/changebotkey")
                {
                    ConsoleCommandsHandler.ChangeTokenAsync();
                }
            }
        }

    }
}
