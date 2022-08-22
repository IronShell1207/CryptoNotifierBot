using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants.Commands
{
    public sealed class BreakoutCommands
    {
        /// <summary>
        /// Добавить пару в черный/белый список.
        /// </summary>
        public static Regex AddToBlackList = new Regex(
            @"/addtoblack\s*((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9}))|)");

        /// <summary>
        /// Добавить топ (кол-во) пар в белый список.
        /// </summary>
        public static Regex AddTopSymbolsToWhiteList = new Regex(@"/addtopwhite\s*(?<count>[0-9]{1,3})");

        /// <summary>
        /// Удалить пару из черного/белого списка.
        /// </summary>
        public static Regex DeleteFav =
            new Regex(
                @"/(delfav)\s*(((?<id>[0-9]+)\s*)|((?<base>[a-zA-Z0-9]{2,9})((\s+|/)(?<quote>[a-zA-Z]{2,9})|)))");

        /// <summary>
        /// Подписаться на breakouts.
        /// </summary>
        public const string Subscribe = "/subscribe";

        /// <summary>
        /// Настройки breakouts.
        /// </summary>
        public const string SubSettings = "/subsets";

        /// <summary>
        /// Отписаться от breakouts.
        /// </summary>
        public const string SubStop = "/substop";

        /// <summary>
        /// Очистить черный список для breakouts.
        /// </summary>
        public const string RemoveAllFromBlackList = "/clearblacklist";

        /// <summary>
        /// Включить тайминги для breakouts.
        /// </summary>
        public static Regex SetTimings = new Regex(@"/setsubtimes\s+(?<timing>[0-9]+)");
    }
}