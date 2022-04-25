using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants
{
    public class Commands
    {
        public const string AllTasks = "/showtasks";
        public const string Subscribe = "/subscribe";
        public const string SubSettings = "/subsets";
        public const string SubStop = "/substop";
    }

    public class AdminCommands
    {
        public static Regex BanUser = new Regex(@"/ban\s*(?<userId>[0-9]*)");
        public static Regex UnbanUser = new Regex(@"/unban\s*(?<userId>[0-9]*)");
    }
}
