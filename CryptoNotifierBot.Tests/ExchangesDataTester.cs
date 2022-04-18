using System;
using System.Collections.Generic;
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
                if (!isValid) Assert.Pass($"{sdata.symbol} {sdata.baseAsset}");
            }
            Assert.Fail();
        }
        [Test]
        public void BinanceApiExGetter()
        {
            var data = BinanceApi.GetExchangeData();
            if (data is SymbolTimedExInfo)
            {
                var sdata = data.Pairs[new Random(23).Next(0,data.Pairs.Count-1)];
                bool isValid = string.IsNullOrEmpty(sdata.Symbol.Name) && string.IsNullOrEmpty(sdata.Symbol.Quote);
                if (!isValid) Assert.Pass($"{sdata.Symbol.ToString()} {sdata.Price.ToString()}");
            }
            Assert.Fail();
        }

        [Test]
        public void OkxApiTester()
        {
            var data = OkxApi.GetExchangeData();
            if (data is SymbolTimedExInfo && data.Pairs.Any())
            {
                var bitcoinprice = data.Pairs.Find(x => x.Symbol.Name == "BTC" && x.Symbol.Quote == "USDT");
                if (bitcoinprice.Price != 0)
                    Assert.Pass($"{bitcoinprice.Symbol.ToString()} {bitcoinprice.Price.ToString()}");

            }

            Assert.Fail();
        }
        [Test]
        public void GateioApiTester()
        {
            var data = GateioApi.GetExchangeData();
            if (data is SymbolTimedExInfo)
            {
                var bitcoinprice = data.Pairs.Find(x => x.Symbol.Name == "BTC" && x.Symbol.Quote == "USDT");
                if (bitcoinprice.Price != 0)
                    Assert.Pass($"{bitcoinprice.Symbol.ToString()} {bitcoinprice.Price.ToString()}");

            }

            Assert.Fail();
        }

        [Test]
        public void KucoinApiTester()
        {
            var data = KucoinAPI.GetExchangeData();
            if (data is SymbolTimedExInfo)
            {
                var BTCPrice = data.Pairs.Find(x => x.Symbol.ToString() == "BTC/USDT");
                if (BTCPrice.Price != 0)
                    Assert.Pass($"{BTCPrice.Symbol.ToString()} {BTCPrice.Price.ToString()}");
            }
            Assert.Fail();
        }

        [Test]
        public void KucoinConverterNullTester()
        {
            List<KucoinData.Ticker> list = null;
            var converted=  KucoinAPI.PairsListConverter(list);
            if (converted != null && converted.Count==0) Assert.Pass();
            Assert.Fail();
        }
        [Test]
        public void OkxConverterNullTester()
        {
            List<OkxPairsInfo> list = null;
            var converted = OkxApi.PairsListConverter(list);
            if (converted != null && converted.Count == 0) Assert.Pass();
            Assert.Fail();
        }
        [Test]
        public void GateioConverterNullTester()
        {
            List<GateIOData> list = null;
            var converted = GateioApi.PairsListConverter(list);
            if (converted != null && converted.Count == 0) Assert.Pass();
            Assert.Fail();
        }

    }
}