using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants.Commands
{
    public sealed class SettingsCommands
    {
        /// <summary>
        /// Установить промежуток уведомлений.
        /// </summary>
        public static Regex ChangeDelay = new Regex(@"/(setinterval|settime|setnotifytimeout) (?<time>[0-9]*)");

        /// <summary>
        /// Установить начало и окончание ночного режима.
        /// </summary>
        public static Regex SetNightTime = new(@"/(setnight)\s+(?<timestart>[0-9:]{4,5})\s+(?<timeend>[0-9:]{4,5})");

        /// <summary>
        /// Включить ночной режим.
        /// </summary>
        public static Regex SetEnableNight = new(@"/enablenight\s*");

        /// <summary>
        /// Установить временную зону.
        /// </summary>
        public static Regex SetTimeZoneCommandRegex = new(@"/timezone\s*(?<time>[-0-9]{1,2})");
    }
}