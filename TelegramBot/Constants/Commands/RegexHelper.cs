using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Constants.Commands
{
    public class RegexHelper
    {
        /// <summary>
        /// Конвертировать сообщение в Regex.
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="args">Аргументы для добавления в regex.</param>
        /// <returns><see cref="Regex"/> из сообщения.</returns>
        public static Regex ConvertMessageToRegex(string message, List<string> args)
        {
            var str = string.Format(message, args.ToArray());
            return new Regex(@str);
        }
    }
}