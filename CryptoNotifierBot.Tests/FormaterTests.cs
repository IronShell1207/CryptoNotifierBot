using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TelegramBot;
namespace CryptoNotifierBot.Tests
{
    
    public class FormaterTests
    {
        [Test]
        public void StringFormaterTest()
        {
           
            var arr = new { pBase = "BTC", pQuote = "USDT", pPrice = 40000, newPrice= 39700};
            string test1 = "{pBase}/{pQuote} {pPrice}->{newPrice}";
            string test2 = "{pBase}-{pQuote} price falls from {pPrice} to {newPrice}";
            string test3 = "{pBase} {pQuote} trigger price: {pPrice} drops to {newPrice}";
            var testresult1 = TelegramBot.Static.StringFormater.StringWithFormat(test1, arr);
            if (testresult1 == "BTC/USDT 40000->39700") Assert.Pass(testresult1);
        }
        [Test]
        public void CryptoObjectFormaterTest()
        {
            var pair = new TelegramBot.Objects.CryptoPair(1)
            {
                Enabled = true,
                ExchangePlatform = "Binance",
                GainOrFall = true,
                PairBase = "BTC",
                PairQuote = "USDT",
                Price = 40000
            };
            var o = new {pair, NewPrice = 39600};
            string formater = "{Enabled} {PairBase}/{PairQuote} {Price} {GainOrFall}";
            var testResult = TelegramBot.Static.StringFormater.StringWithFormat(formater, pair);
            if (testResult == "True BTC/USDT 40000 True") Assert.Pass(testResult);
            Assert.Fail(testResult);
        }

        [Test]
        public void ResxTester()
        {
            var test1= TelegramBot.Static.MessagesGetter.GetGlobalString("newPairWrongPrice", "en");
            if (test1== "Wrong price. Try again") Assert.Pass(test1);
            
        }

        [Test]
        public void Resx2Tester()
        {
            var test2 = TelegramBot.Static.MessagesGetter.GetGlobalString("newPairWrongPrice", "ru");
            if (test2 == "Неверная цена. Попробуйте снова") Assert.Pass(test2);
        }
    }
}
