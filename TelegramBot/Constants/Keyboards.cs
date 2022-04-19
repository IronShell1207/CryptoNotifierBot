using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Objects;

namespace TelegramBot.Constants
{
    public class Keyboards
    {
        public static InlineKeyboardMarkup ExchangeSelectingKeyboardMarkup(List<string> exchanges)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            foreach (string exchangesItem in exchanges)
            {
                InlineKeyboardButton button = new InlineKeyboardButton(exchangesItem);
                button.CallbackData = exchangesItem;
                buttons.Add(button);
            }

            return new InlineKeyboardMarkup(buttons.ToArray());
        }

        public static InlineKeyboardMarkup PairsSelectingKeyboardMarkup(List<CryptoPair> pairs, string dataPattern)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            foreach (CryptoPair pair in pairs)
            {
                var btnText = pair.TaskStatus();
                InlineKeyboardButton btn = new InlineKeyboardButton(btnText);
                btn.CallbackData = string.Format(dataPattern, arg0: pair.Id, arg1: pair.OwnerId);
                buttons.Add(btn);
            }

            return new InlineKeyboardMarkup(buttons.ToArray());
        }
    }
}
