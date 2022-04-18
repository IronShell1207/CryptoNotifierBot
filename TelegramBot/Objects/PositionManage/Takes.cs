using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class Takes
    {
        public int Id { get; set; }
        /// <summary>
        /// Owner here is a position pair object id
        /// </summary>
        public int OwnerId { get; set; }
        public double Price { get; set; }
        public bool Triggered { get; set; } = false;

    }
}
