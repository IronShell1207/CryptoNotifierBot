using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.Write("Set telegram bot token: ");
            var reader = Console.ReadLine();
            var token = RegexCombins.TelegramBotToken.Match(reader);
            if (token.Success)
            {
                AppSettingsStatic.Settings.TelegramBotToken = token.Value;
                Console.WriteLine("Bot token saved!");
                JsonHelper.SaveJson(AppSettingsStatic.Settings, "config.json");
                Console.WriteLine("You need to restart program to apply changes!");
            }
            else
            {
                Console.WriteLine("Token invalid. Try again");
                goto setbottoken;
            }
        }

        public static async Task StartBotAsync()
        {
            Console.WriteLine("Starting telegram bot...");
            Console.WriteLine("Connecting to telegram api...");
            await Task.Run(BotApi.StartBotAsync);
            var botInfo = await BotApi.BotClient.GetMeAsync();
            Console.WriteLine($"Bot connected! Bot info: {botInfo}");
        }

        public static async Task SetAdminId()
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
    }
}
