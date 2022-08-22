using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants.Commands
{
    public class MonitoringTaskCommands
    {
        public static Regex CreatePair = new(
            @"/(new|create)\s*((?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})\s*(?<price>([0-9.]+)|)|)");

        public static Regex EditPair = new(
            @"/(edit)\s*((((?<id>[0-9]+)\s*((?<price>[0-9.e]*)|))|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");

        public static Regex DeletePair =
            new(
                @"/(delete|remove)\s*((((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");

        public static Regex ShowPair =
            new(
                @"/(show|display)\s*((((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))|)");

        public static Regex TriggerOncePair =
            new(
                @"/(trones|triggeronce|oneshot)\s*((?<id>[0-9]+)\s*|)");

        public static Regex ShiftTasks = new(@"(/movetasks)\s*((?<procent>[0-9]+)|(?<create>true)|)");
        public static Regex AddComment = new(@"/((addcom)|(addcomment))\s*(?<id>[0-9]*)");

        public List<string> Commands
        {
            get
            {
                return new List<string> { "/new", "/edit", "/delete", "/movetasks" };
            }
        }
    }
}