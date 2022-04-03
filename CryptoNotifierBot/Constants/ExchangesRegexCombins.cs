using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptoApi.Constants
{
    public class ExchangesRegexCombins
    {
       public static Regex cryptoSymbol = new Regex(@"(?<name>[A-Z0-9]{2,7})(?<quote>(ETH|BUSD|BTC|USDT|BNB))");

    }
}
