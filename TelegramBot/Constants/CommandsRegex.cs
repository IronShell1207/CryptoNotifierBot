using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants
{
    public class CommandsRegex
    {
        public static Regex ConvertMessageToRegex(string message, List<string> args)
        {
            var str = string.Format(message, args.ToArray());
            return new Regex(@str);
        }
        public class MonitoringTaskCommands
        {
            public static Regex CreatePair = new Regex(
                @"/(new|create)\s*((?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})\s*(?<price>([0-9.]+)|)|)");

            public static Regex EditPair = new Regex(
                @"/(edit)\s*(((?<id>[0-9]+)\s*((?<price>[0-9.e]*)|))|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))");

            public static Regex DeletePair =
                new Regex(
                    @"/(delete|remove)\s*(((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))");
            public List<string> Commands
            {
                get
                {
                    return new List<string> { "/new", "/edit", "/delete" };
                }
            }
        }

        public class BreakoutCommands
        {

            public static Regex AddToBlackList = new Regex(
                @"/addtoblack\s*((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9}))|)");

            public static Regex DeleteFav =
                new Regex(
                    @"/(delfav)\s*(((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))");

            public List<string> Commands
            {
                get
                {
                    return new List<string> { "/addtoblack", "/delfav" };
                }
            }
        }

        public class SettingsCommands
        {
            public static Regex ChangeDelay = new Regex(@"/settimeout (?<time>[0-9]*)");
        }

        public static Regex SetTimings = new Regex(@"/setsubtimes\s+(?<timing>[0-9]+)");
        
        //public static Regex CreatePosition = new Regex(@"/(newpos)\s*((?<base>[a-zA-Z0-9]{2,9})(\s+(?<quote>[a-zA-Z]{2,9})|))");
        //public static Regex DeletePosition = new Regex(@"/(delpos)\s*(((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})(\s+(?<quote>[a-zA-Z]{2,9})|)))");
        //public static Regex EditPosition = new Regex(@"");
    }
}
