using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CryptoApi.Static.DataHandler;
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
        public static DataRequester cryptoData { get; private set; }
        static void Main(string[] args)
        {
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            ConsoleCommandsHandler.LogWrite("Crypto notifications bot loading...");
            if (string.IsNullOrWhiteSpace(AppSettingsStatic.Settings.TelegramBotToken))
            {
                ConsoleCommandsHandler.ChangeTokenAsync();
            }
            ConsoleCommandsHandler.StartBotAsync();
            if (string.IsNullOrWhiteSpace(AppSettingsStatic.Settings.AdminTelegramId))
            {
                ConsoleCommandsHandler.SetAdminId().Wait();
            }

            ConsoleCommandsHandler.LogWrite($"Bot has been started!");
            cryptoData = new DataRequester();
            Task.Run(cryptoData.UpdateDataLoop);
            while (!cryptoData.DataAvailable)
            { 
                Thread.Sleep(1000);
                ConsoleCommandsHandler.LogWrite($"Awaiting when data will available...");
            }
            Task.Run(() => NotifyLoops.MainLoop());
            ConsoleCommandsHandler.LogWrite("Crypto exchanges data updater loop has been started");
            while (true)
            {
                try
                {
                    var reader = Console.ReadLine();
                    if (reader.ToString() == "/changebotkey" || reader.ToString() == "/setbotkey")
                    {
                        ConsoleCommandsHandler.ChangeTokenAsync();
                    }
                    else if (reader.ToString() == "/exit")
                        Environment.Exit(0);
                }
                catch (Exception ex) {}
            }
        }
        private static async void Awaiter(){}

    }
}
