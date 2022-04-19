using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TelegramBot;
using TelegramBot.Constants;

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

        [Test]
        public void MessageToRegexFormater()
        {
            var teststroke = "To edit price of {0} with id {1} send new price with this message attached!";
            var testRusStroke = "Чтобы изменить цену {0} с id {1} отправь новую цену с прикрепленным сообщением!";
            var testRusStrokeResult = "Чтобы изменить цену BTC/USDT с id 1231 отправь новую цену с прикрепленным сообщением!";
            var resultstroke = "To edit price of BTC/USDT with id 1231 send new price with this message attached!";
            string pair = "BTC/USDT";
            int id = 1231;
            var paramsList = new List<string>(){ @"(?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})", "(?<id>[0-9]+)"};
            var reRegx = CommandsRegex.ConvertMessageToRegex(teststroke, paramsList);
            var reRegxRus = CommandsRegex.ConvertMessageToRegex(testRusStroke, paramsList);
            var match = reRegxRus.Match(testRusStrokeResult);
            if (match.Success)
            {
                var pairRes = match.Groups["base"].Value;
                var pairQRes = match.Groups["quote"].Value;
                var pairR = $"{pairRes}/{pairQRes}";
                var idRes = int.Parse(match.Groups["id"].Value);
                if (pairR == pair && id == idRes) Assert.Pass(pairRes);
            }
            Assert.Fail();
        }
    }
}
