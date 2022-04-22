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
            var binanceApi = new BinanceApi();
            var data = binanceApi.GetFullData();
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
            var binanceApi = new BinanceApi();
            var data = binanceApi.GetExchangeData();
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
            var okxApi = new OkxApi();
            var data = okxApi.GetExchangeData();
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
            var gateApi = new GateioApi();
            var data = gateApi.GetExchangeData();
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
            var kucoapi = new KucoinAPI();
            var data = kucoapi.GetExchangeData();
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
            var kucoapi = new KucoinAPI();
            List<KucoinData.Ticker> list = null;
            var converted= kucoapi.PairsListConverter(list);
            if (converted != null && converted.Count==0) Assert.Pass();
            Assert.Fail();
        }
        [Test]
        public void OkxConverterNullTester()
        {
            var okxApi = new OkxApi();
            List<OkxPairsInfo> list = null;
            var converted = okxApi.PairsListConverter(list);
            if (converted != null && converted.Count == 0) Assert.Pass();
            Assert.Fail();
        }
        [Test]
        public void GateioConverterNullTester()
        {
            var gateapi = new GateioApi();
            List<GateIOData> list = null;
            var converted = gateapi.ExchangePairsConverter(list);
            if (converted != null && converted.Count == 0) Assert.Pass();
            Assert.Fail();
        }

        [Test]
        public void BitgetDataTester()
        {
            using (var bitgetApi = new BitgetApi())
            {
                var data= bitgetApi.GetExchangeData();
                if (data is SymbolTimedExInfo && data.Pairs.Any())
                {
                    var bitcoin = data.Pairs.FirstOrDefault(x => x.Symbol.ToString() == "BTC/USDT");
                    var btcprice = bitcoin.Price;
                    if (btcprice > 0) Assert.Pass(btcprice.ToString());
                    
                }
                else Assert.Fail(data.ToString());
            }
        }

    }
}