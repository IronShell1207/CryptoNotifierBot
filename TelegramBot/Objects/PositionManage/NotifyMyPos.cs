    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class NotifyMyPos
    {
        public int Id { get; set; }
        /// <summary>
        /// Owner here is a position pair object id
        /// </summary>
        public int OwnerId { get; set; }

        public double ProcentNotify { get; set; }
    }
}
