using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CryptoApi;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Static;
using Microsoft.Data.Sqlite;
using Telegram.Bot.Types;
using TelegramBot.Objects;
using TelegramBot.Static.BotLoops;
using CryptoPair = TelegramBot.Objects.CryptoPair;

namespace TelegramBot.Static
{
    public class NotifyLoops
    {
        public static void MainLoop()
        {
            Task.Run(BreakoutMonitor.BreakoutLoop);
            Task.Run(BotLoops.MonitorLoop.Loop);
            Task.Run(MorningReport.MorningSpread);
        }
    }
}