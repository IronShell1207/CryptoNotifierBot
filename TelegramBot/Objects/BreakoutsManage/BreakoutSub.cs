using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public bool WhitelistInsteadBlack { get; set; } = false;
        public bool Subscribed { get; set; } = true;
        #region Exchanges
        public bool GateioSub { get; set; } = true;
        public bool BinanceSub { get; set; } = true;
        public bool KucoinSub { get; set; } = true;
        public bool BitgetSub { get; set; } = true;
        public bool OkxSub { get; set; } = true;
        #endregion
        #region Timings
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
#endregion
        public List<BlackListedPairs>? BlackListedPairsList { get; set; } = new();

        //[ForeignKey(nameof(UserConfig))]
        public int UserId { get; set; }
        //public UserConfig? UserConfig { get; set; }

        public bool AreEqual(BreakoutSub inc)
        {
            return (inc.Id == Id 
                && inc.TelegramId == TelegramId
                && inc.BinanceSub == BinanceSub
                && inc.BitgetSub == BitgetSub
                && inc.BlackListEnable == BlackListEnable
                && inc.GateioSub == GateioSub
                && inc.OkxSub == OkxSub
                && inc.KucoinSub == KucoinSub
                && inc.S120MinUpdates == S120MinUpdates
                && inc.S15MinUpdates == S15MinUpdates
                && inc.S1920MinUpdates == S1920MinUpdates
                && inc.S240MinUpdates == S240MinUpdates
                && inc.S2MinUpdates == S2MinUpdates
                && inc.S45MinUpdates == S45MinUpdates
                && inc.S30MinUpdates == S30MinUpdates
                && inc.WhitelistInsteadBlack == WhitelistInsteadBlack
                && inc.UserId == UserId
                && inc.S960MinUpdates == S960MinUpdates
                && inc.S60MinUpdates == S60MinUpdates
                && inc.S5MinUpdates == S5MinUpdates
                && inc.Subscribed == Subscribed);
        }
    }
}
