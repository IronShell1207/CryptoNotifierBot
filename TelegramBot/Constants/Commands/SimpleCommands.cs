using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants.Commands
{
    public class SimpleCommands
    {
        /// <summary>
        /// Показать все задания.
        /// </summary>
        public const string AllTasks = "/showtasks";

        /// <summary>
        /// Перевернуть задания на отслеживание цены с созданием заданий.
        /// </summary>
        public const string FlipTasks = "/fliptasks";

        /// <summary>
        /// Передвинуть цены сработавших отслеживаний.
        /// </summary>
        public const string ShiftTasks = "/movetasks";

        /// <summary>
        /// Очистить черный список для
        /// </summary>
        public const string RemoveAllFromBlackList = "/clearblacklist";

        /// <summary>
        /// Показать настройки.
        /// </summary>
        public const string ShowSettings = "/showsetts";

        /// <summary>
        /// Удалять последнее сообщение автоматически.
        /// </summary>
        public const string EnableCleaning = "/cleanlast";
    }

    public class AdminCommands
    {
        public static Regex BanUser = new Regex(@"/ban\s*(?<userId>[0-9]*)");
        public static Regex UnbanUser = new Regex(@"/unban\s*(?<userId>[0-9]*)");
    }
}