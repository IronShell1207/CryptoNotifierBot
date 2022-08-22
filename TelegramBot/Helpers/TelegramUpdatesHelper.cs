using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace TelegramBot.Helpers
{
    public sealed class TelegramUpdatesHelper
    {
        /// <summary>
        /// Взять ID пользователя из update.
        /// </summary>
        /// <param name="update">Telegram bot update</param>
        /// <returns>Chat id.</returns>
        public static ChatId GetTelegramIdFromUpdate(Update update)
        {
            if (update.Message?.Chat?.Id != null)
                return update.Message.Chat.Id;
            else if (update.MyChatMember?.From.Id != null)
                return new ChatId((long)update.MyChatMember?.From?.Id);
            else if (update.MyChatMember?.Chat.Id != null)
                return new ChatId((long)update.MyChatMember?.Chat?.Id);
            else if (update.CallbackQuery?.From?.Id != null)
                return update.CallbackQuery.From.Id;
            else if (update.EditedMessage?.From?.Id != null)
                return update.EditedMessage.From.Id;
            else return null;
        }

        /// <summary>
        /// Получить message id полученного сообщения.
        /// </summary>
        public static async Task<int> GetMessageIdFromUpdateTask(Update update)
        {
            if (update.Message?.MessageId != null) return update.Message.MessageId;
            else if (update.CallbackQuery?.Message?.MessageId != null) return update.CallbackQuery.Message.MessageId;
            else return 0;
        }

        public static string StripHTML(string input)
        {
            input = input.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}