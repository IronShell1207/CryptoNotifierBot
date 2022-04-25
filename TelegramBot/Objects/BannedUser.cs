using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class BannedUser
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string BanReason { get; set; }
    }
}
