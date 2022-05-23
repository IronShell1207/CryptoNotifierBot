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
            var listButns = new List<List<InlineKeyboardButton>>();
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            for (int i = 0; i < pairs.Count; i++)
            {
                
                var btnText = pairs[i].TaskStatus();
                InlineKeyboardButton btn = new InlineKeyboardButton(btnText);
                btn.CallbackData = string.Format(dataPattern, arg0: pairs[i].Id, arg1: pairs[i].OwnerId);
                buttons.Add(btn);
                if ((i % 2) == 1)
                {
                    listButns.Add(buttons);
                    buttons = new List<InlineKeyboardButton>();
                }
            }

            return new InlineKeyboardMarkup(listButns);
        }
    }
}
