using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace TelegramBot.Objects
{
    public class BreakoutSub
    {
        
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public bool Subscribed { get; set; }
        public bool GateioSub { get; set; } 
        public bool BinanceSub { get; set; } 
        public bool KucoinSub { get; set; }
        public bool OkxSub { get; set; } 
        public bool S5MinUpdates { get; set; }
        public bool S2MinUpdates { get; set; } 
        public bool S15MinUpdates { get; set; } 
        public bool S30MinUpdates { get; set; } 
        public bool S45MinUpdates { get; set; }
        public bool S60MinUpdates { get; set; } 
        public bool S120MinUpdates { get; set; }
        public bool S240MinUpdates { get; set; } 
        public bool S480MinUpdates { get; set; }
        public bool S960MinUpdates { get; set; } 
        public bool S1920MinUpdates { get; set; }

    }
}
