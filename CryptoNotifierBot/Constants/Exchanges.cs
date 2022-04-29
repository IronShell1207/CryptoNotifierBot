using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Constants
{
    public class Exchanges
    {
        public const string GateIO = "GateIO";
        public const string Okx = "Okx";
        public const string Binance = "Binance";
        public const string Kucoin = "Kucoin";
        public const string Bitget = "Bitget";
        public static bool Contains(string name)
        {
            if (name is GateIO or Okx or Binance or Kucoin)
                return true;
            return false;   
        }

        public static List<string> ExchangeList = new List<string>()
        {
            GateIO,Okx,Bitget,Binance,Kucoin
        };
    }
}
