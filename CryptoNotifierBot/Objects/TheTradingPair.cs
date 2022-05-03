using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public class TheTradingPair
    {
        public virtual string Symbol { get; }
        public virtual string Price { get; }
    }
}
