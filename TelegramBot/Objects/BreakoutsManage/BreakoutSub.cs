using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        public bool BlackListEnable { get; set; } = false;
        public bool Subscribed { get; set; } = true;
        public bool GateioSub { get; set; } = true;
        public bool BinanceSub { get; set; } = true;
        public bool KucoinSub { get; set; } = true;
        public bool BitgetSub { get; set; } = true;
        public bool OkxSub { get; set; } = true;
        public bool S5MinUpdates { get; set; } = true;
        public bool S2MinUpdates { get; set; } = false;
        public bool S15MinUpdates { get; set; } = true;
        public bool S30MinUpdates { get; set; } = false;
        public bool S45MinUpdates { get; set; } = false;
        public bool S60MinUpdates { get; set; } = true;
        public bool S120MinUpdates { get; set; } = false;
        public bool S240MinUpdates { get; set; } = false;
        public bool S480MinUpdates { get; set; } = true;
        public bool S960MinUpdates { get; set; } = false;
        public bool S1920MinUpdates { get; set; } = true;
        public List<BlackListedPairs>? BlackListedPairsList { get; set; } = new();

    }
}
