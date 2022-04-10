using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class PositionPair
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Base { get; set; }
        public string Quote { get; set; }
        public double Entry { get; set; }
        public double StopLoss { get; set; }
    }
}
