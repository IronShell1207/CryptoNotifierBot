using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public sealed class MonObj
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string PairBase { get; set; }

        public MonObj(string pairBase)
        {
            PairBase = pairBase;
        }
        public override string ToString() => PairBase;

        public MonObj()
        {

        }
    }
}
