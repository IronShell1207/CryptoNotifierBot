using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects.ExchangesPairs;

namespace CryptoApi.Constants
{
    public class Exchanges
    {
        public const string GateIO = "GateIO";
        public const string Okx = "Okx";
        public const string Binance = "Binance";
        public const string Kucoin = "Kucoin";
        public const string Bitget = "Bitget";
        public const string Mexc = "Mexc";
        private const string CoinEx = "CoinEx";
        public static bool Contains(string name)
        {
            if (name is GateIO or Okx or Binance or Kucoin)
                return true;
            return false;   
        }

        public static string ExApiSeparator(string name)
        {
            switch (name)
            {
                case GateIO:
                case Mexc:
                    return "_";

                case Okx:
                case Kucoin:
                    return "-";

                case Binance:
                case Bitget:
                    return null;
                
                default: return "_";
            }
        }

        public static Type ExDataType(string name)
        {
            switch (name)
            {
                case GateIO:
                    return typeof(GateIOTicker);
                case Okx:
                    return typeof(OkxData);
                case Binance:
                    return typeof(BinancePair);
                case Bitget:
                    return typeof(BitgetData);
                case Kucoin:
                    return typeof(KucoinData);
                case Mexc:
                    return typeof(MexcData);
                default: return null;
            }
        }

        public static List<string> ExchangeList = new List<string>()
        {
            GateIO,Okx,Bitget,Binance,Kucoin, Mexc
        };
    }
}
