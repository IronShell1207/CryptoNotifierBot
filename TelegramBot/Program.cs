﻿using System;
using System.Globalization;
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
            
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Bot has been started!");
            cryptoData = new DataRequester();
            Task.Run(cryptoData.UpdateDataLoop);
            Task.Run(() => NotifyLoops.MainLoop());
            while (true)
            {
                var reader = Console.ReadLine();
                if (reader.ToString() == "/changebotkey" || reader.ToString() == "/setbotkey")
                {
                    ConsoleCommandsHandler.ChangeTokenAsync();
                }
            }
        }

    }
}
