using System;
using System.Linq;
using System.Net.NetworkInformation;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using CryptoApi.Static;
using NUnit.Framework;
using CryptoApi;
using CryptoApi.Constants;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace CryptoApi.Tests
{
    public class ExchangesDataTester
    {

        [Test]
        public void BinanceApiFullInfoGetter()
        {
            var data = BinanceApi.GetFullData();
            if (data is BinanceSymbolsData)
            {
                var sdata = data.symbols?[new Random(2).Next(0, data.symbols.Length-1)]; 
                bool isValid = string.IsNullOrEmpty(sdata.baseAsset) && string.IsNullOrEmpty(sdata.symbol);
                if (!isValid) Assert.Pass();
            }
            Assert.Fail();
        }
        [Test]
        public void BinanceApiExGetter()
        {
            var data = BinanceApi.GetExchangeInfo();
            if (data is SymbolTimedExInfo)
            {
                var sdata = data.Pairs[new Random(23).Next(0,data.Pairs.Count-1)];
                bool isValid = string.IsNullOrEmpty(sdata.Symbol.Name) && string.IsNullOrEmpty(sdata.Symbol.Quote);
                if (!isValid) Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public void OkxApiTester()
        {
            var data = OkxApi.GetExchangeInfo();
            if (data is SymbolTimedExInfo && data.Pairs.Any())
            {
                var bitcoinprice = data.Pairs.Find(x => x.Symbol.Name == "BTC" && x.Symbol.Quote == "USDT");
                if (bitcoinprice.Price != 0)
                    Assert.Pass();

            }

            Assert.Fail();
        }
        [Test]
        public void GateioApiTester()
        {
            var data = GateioApi.GetExchangeInfo();
            if (data is SymbolTimedExInfo)
            {
                var bitcoinprice = data.Pairs.Find(x => x.Symbol.Name == "BTC" && x.Symbol.Quote == "USDT");
                if (bitcoinprice.Price != 0)
                    Assert.Pass();

            }

            Assert.Fail();
        }

    }
}