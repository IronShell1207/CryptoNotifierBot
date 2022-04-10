using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

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
    }
}
