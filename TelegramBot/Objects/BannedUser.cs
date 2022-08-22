using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class BannedUser
    {
        public int Id { get; set; }
        public long? TelegramId { get; set; }
        public string? BanReason { get; set; }
        public UserConfig? User { get; set; }
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public string? UserName { get; set; } 
    }
}
