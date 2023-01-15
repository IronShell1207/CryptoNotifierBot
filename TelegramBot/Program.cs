using CryptoApi.Static.DataHandler;
using System.Globalization;
using TelegramBot.Constants;
using TelegramBot.Static;

namespace TelegramBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
        }

        /* private static void Main(string[] args)
         {
             Console.CursorVisible = false;
             CultureInfo ci = new CultureInfo("en");
             Thread.CurrentThread.CurrentCulture = ci;
             Botlogo.PrintLogo(Botlogo.LogoMain);
             cryptoData = new DataRequester();
             Task.Run(cryptoData.UpdateDataLoop);
             ConsoleSpinner spinner = new ConsoleSpinner();
             Console.WriteLine($"Awaiting when data will available...");

             using (AppDbContext dbContext = new()) dbContext.MigrateStart();
             while (!cryptoData.DataAvailable)
             {
                 spinner.UpdateProgress();
                 Thread.Sleep(240);
             }
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
             Console.WriteLine($"Bot has been started!");
             Task.Run(() => NotifyLoops.MainLoop());
             Console.WriteLine("Crypto exchanges data updater loop has been started");
             while (true)
             {
                 Console.CursorVisible = true;
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
                 catch (Exception ex) { }
             }
         }

         private static async void Awaiter()
         { }*/
    }
}