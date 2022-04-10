using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    internal class Takes
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public double Price { get; set; }
        public bool Triggered { get; set; } = false;

    }
}
