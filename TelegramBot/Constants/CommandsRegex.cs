using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants
{
    public class CommandsRegex
    {
        public static Regex AddPairsCommandRegex =
            new(
                @"/(?<command>\w*)\s*((?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})\s*(?<price>([0-9.]+)|)|)");
        public static Regex AddMonPairsCommandRegex =
            new(
                @"/addmon\s*(?<base>[a-zA-Z0-9]{2,9})");
        public static Regex SetTimeZoneCommandRegex =
            new(
                @"/timezone\s*(?<time>[-0-9]{1,2})");
        public static Regex DelMonPairsCommandRegex =
            new(
                @"/delmon\s*(?<base>[a-zA-Z0-9]{2,9})");
        public static Regex EditPairsCommandRegex =
            new(
                @"/(?<command>/w*)\s*((((?<id>[0-9]+)\s*((?<price>[0-9.e]*)|))|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");

        public static Regex ConvertMessageToRegex(string message, List<string> args)
        {
            var str = string.Format(message, args.ToArray());
            return new Regex(@str);
        }
        public class MonitoringTaskCommands
        {
            
            public static Regex CreatePair = new (
                @"/(new|create)\s*((?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})\s*(?<price>([0-9.]+)|)|)");

            public static Regex EditPair = new (
                @"/(edit)\s*((((?<id>[0-9]+)\s*((?<price>[0-9.e]*)|))|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");

            public static Regex DeletePair =
                new (  
                    @"/(delete|remove)\s*((((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");
            public static Regex ShowPair =
                new (
                    @"/(show|display)\s*((((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");
            public static Regex TriggerOncePair =
                new (
                    @"/(trones|triggeronce|oneshot)\s*((?<id>[0-9]+)\s*|)");

            public static Regex ShiftTasks = new (@"(/movetasks)\s*((?<procent>[0-9]+)|(?<create>true)|)");
            public static Regex AddComment = new (@"/((addcom)|(addcomment))\s*(?<id>[0-9]*)");

            public List<string> Commands
            {
                get
                {
                    return new List<string> { "/new", "/edit", "/delete", "/movetasks" };
                }
            }
        }

        public class BreakoutCommands
        {

            public static Regex AddToBlackList = new Regex(
                @"/addtoblack\s*((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9}))|)");

            public static Regex AddTopSymbolsToWhiteList = new Regex(@"/addtopwhite\s*(?<count>[0-9]{1,3})");

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
            public static Regex ChangeDelay = new Regex(@"/(setinterval|settime|setnotifytimeout) (?<time>[0-9]*)");
            public static Regex SetNightTime = new(@"/(setnight)\s+(?<timestart>[0-9:]{4,5})\s+(?<timeend>[0-9:]{4,5})");
            public static Regex SetEnableNight = new(@"/enablenight\s*");
        }

        public static Regex SetTimings = new Regex(@"/setsubtimes\s+(?<timing>[0-9]+)");
        
        //public static Regex CreatePosition = new Regex(@"/(newpos)\s*((?<base>[a-zA-Z0-9]{2,9})(\s+(?<quote>[a-zA-Z]{2,9})|))");
        //public static Regex DeletePosition = new Regex(@"/(delpos)\s*(((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})(\s+(?<quote>[a-zA-Z]{2,9})|)))");
        //public static Regex EditPosition = new Regex(@"");
    }
}
