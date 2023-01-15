using System.Text.RegularExpressions;

namespace TelegramBot.Commands
{
    public class UserSettingsCommand
    {
        public static readonly Regex DisableNotifications = new Regex(@"/disablenotifications");
        public static readonly Regex ChangeNotificationsDelay = new Regex(@"/setinterval");

        /// <summary>
        /// Устанавить ночное время.
        /// </summary>
        public static readonly Regex SetNightTime = new Regex(@"/setnightmodetimes\s+(?<timestart>[0-9:]{4,5})\s+(?<timeend>[0-9:]{4,5})");

        /// <summary>
        /// Включение или отключение ночного режима.
        /// </summary>
        public static readonly Regex ChangeNightStatus = new Regex(@"/enablenightmode");

        public static readonly Regex SetTimeZone = new Regex(@"/timezone\s*(?<time>[-0-9]{1,2})");
    }
}