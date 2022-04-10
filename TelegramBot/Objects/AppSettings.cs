using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class AppSettings
    {
        public string TelegramBotToken { get; set; }
        public string AdminTelegramId { get; set; }

        public string DbConnectionString { get; set; } = "Filename=telegrambot.db";
        // @"Data Source=(localdb)\MSSQLLocalDB;User id=sa;Password=00000aaa;Initial Catalog=CryptoNotifyBot;Connect Timeout=12;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


    }
}
