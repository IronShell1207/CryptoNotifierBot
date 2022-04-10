using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Constants
{
    public class Exchanges
    {
        public static string GateIO = "GateIO";
        public static string Okx = "Okx";
        public static string Binance = "Binance";
        public static string Kucoin = "Kucoin";

        public static bool Contains(string name)
        {
            if (name == GateIO || name == Okx || name == Binance || name == Kucoin)
                return true;
            return false;   
        }
    }
}
