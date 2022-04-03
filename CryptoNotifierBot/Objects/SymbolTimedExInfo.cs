using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public class SymbolTimedExInfo
    {
        public List<CryptoExchangePairInfo> Pairs { get; set; } = new List<CryptoExchangePairInfo>();
        public DateTime CreationTime { get; set; } =DateTime.Now;
    }
}
