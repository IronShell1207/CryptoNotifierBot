using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace TelegramBot.Constants
{
    internal class Botlogo
    {
        public static string[] LogoMain = new[]
        {
@"   ___                      _____ _           _ ",
@"  / __\ __ _   _  /\/\   __/__   \ |__   ___ | |_",
@" / / | '__| | | |/    \ / _ \/ /\/ '_ \ / _ \| __|",
@"/ /__| |  | |_| / /\/\ \  __/ /  | |_) | (_) | |_ ",
@"\____/_|   \__, \/    \/\___\/   |_.__/ \___/ \__|",
"           |___/                                  "
        };
        public static void PrintLogo(string[] logo)
        {
            foreach (string logoItem in logo)
            {
                Console.WriteLine(logoItem);
                Thread.Sleep(30);
            }
        }
    }
}
