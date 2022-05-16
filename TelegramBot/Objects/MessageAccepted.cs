using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class MessageAccepted
    {
        public int Id { get; set; }
        public UserConfig? UserConfig { get; set;}
        public int UserConfigId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int? MessageId { get; set; }
        public MessageAccepted(){}

        public MessageAccepted(UserConfig user, string messageText, int messageId =0)
        {
            UserConfig = user;
            Text = messageText;
            MessageId = messageId;
        }
    }
}
