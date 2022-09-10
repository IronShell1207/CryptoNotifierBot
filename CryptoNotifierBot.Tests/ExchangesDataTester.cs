using System;
using System.Collections.Generic;
using CryptoApi.Static;
using CryptoApi.Static.DataHandler;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CryptoApi.API;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CryptoApi.Tests
{
    public class ExchangesDataTester
    {
        private Guid gid = Guid.NewGuid();

        [Test]
        public async Task BitgetTest()
        {
            using (BitgetApi API = new BitgetApi())
                await API.GetExchangeData(gid);
            Assert.Pass(await GetBtcPrice(Exchanges.Bitget));

        }

        [Test]
        public async Task BinanceTest()
        {
            using (BinanceApi API = new BinanceApi())
                await API.GetExchangeData(gid);
            Assert.Pass(await GetBtcPrice(Exchanges.Binance));
        }

        [Test]
        public async Task OkxTest()
        {
            using (OkxApi API = new OkxApi())
                await API.GetExchangeData(gid);
            Assert.Pass(await GetBtcPrice(Exchanges.Okx));
        }

        [Test]
        public async Task KucoinTest()
        {
            using (KucoinAPI API = new KucoinAPI())
                await API.GetExchangeData(gid);
            Assert.Pass(await GetBtcPrice(Exchanges.Kucoin));
        }

        [Test]
        public async Task GateioTest()
        {
            using (GateioApi API = new GateioApi())
                await API.GetExchangeData(gid);
            Assert.Pass(await GetBtcPrice(Exchanges.GateIO));
        }

        public async Task<string> GetBtcPrice(string exchange)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var dbset = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == exchange);
                var pairs = dbContext.TradingPairs.Where(x => x.CryDbSetId == dbset.Id);
                var btc = pairs.First(x => x.Name == "BTC" && x.Quote == "USDT");
                if (btc?.CryDbSetId == dbset.Id)
                    return $"{btc.ToString()}: {btc.Price} created in {dbset.DateTime.ToString()} from {btc.Exchange}";
                else Assert.Fail(pairs.Count().ToString());
            }

            return "";
        }

        [Test]
        public async Task GetCurrentPrice()
        {
            using (DataRequester dreq = new DataRequester())
            {
                var pair = new TradingPair("ETH", "USDT");
                // dreq.GetCurrentPricePairByName()

            }
        }



        [Test]
        public async Task GetBtcPrice2()
        {
            var btcusdt = new TradingPair("BTC", "USDT");
            var expectedPrice = 29000;
            using (DataRequester dreq = new DataRequester())
            {
                var priced = await dreq.GetCurrentPricePairByName(btcusdt);
                var getDbSet = priced.CryDbSet;
                if (priced.Price > expectedPrice)
                    Assert.Pass(
                        $"{priced.ToString()}: {priced.Price} from {priced.Exchange} loaded in {getDbSet.DateTime.ToString()}");
            }


        }



        [Test]
        public async Task ZGetLatestDataSets()
        {
            using (DataRequester dreq = new DataRequester())
            {
                var data = await dreq.GetLatestDataSets(45);
                if (data.Count == Exchanges.ExchangeList.Count) Assert.Pass(data.Count.ToString());
                else Assert.Fail(data.Count.ToString());
            }
        }

        [Test]
        public async Task ZGetExchangesForPairTest()
        {
            using (DataRequester dreq = new DataRequester())
            {
                var pair = new TradingPair("ETH", "USDT");
                var exh = await dreq.GetExchangesForPair(pair);
                if (exh.Count == Exchanges.ExchangeList.Count) Assert.Pass(exh.Count.ToString());
                else Assert.Fail(exh.Count.ToString());
            }
        }

        private Guid NewGuid = Guid.NewGuid();
        [Test]
        public async Task NewGateIoTest()
        {
            using (CryptoApi.API.ExchangeApi api = new ExchangeApi(Exchanges.GateIO))
            {
                await api.GetExchangeData<List<GateIOTicker>>(NewGuid);
                Assert.Pass(await GetBtcPrice(Exchanges.GateIO));
            }
        }

        [Test]
        public async Task NewBinanceTest()
        {
            using (CryptoApi.API.ExchangeApi api = new ExchangeApi(Exchanges.Binance))
            {
                await api.GetExchangeData<List<BinancePair>>(NewGuid);
                Assert.Pass(await GetBtcPrice(Exchanges.Binance));
            }
        }
        [Test]
        public async Task NewOkxTest()
        {
            using (CryptoApi.API.ExchangeApi api = new ExchangeApi(Exchanges.Okx))
            {
                await api.GetExchangeData<OkxData>(NewGuid);
                Assert.Pass(await GetBtcPrice(Exchanges.Okx));
            }
        }

        [Test]
        public async Task NewKucoinTest()
        {
            using (CryptoApi.API.ExchangeApi api = new ExchangeApi(Exchanges.Kucoin))
            {
                await api.GetExchangeData<KucoinData>(NewGuid);
                Assert.Pass(await GetBtcPrice(Exchanges.Kucoin));
            }
        }

        [Test]
        public async Task NewBitgetTest()
        {
            using (ExchangeApi api = new ExchangeApi(Exchanges.Bitget))
            {
                await api.GetExchangeData<BitgetData>(NewGuid);
                Assert.Pass(await GetBtcPrice(Exchanges.Bitget));
            }
        }

        [Test]
        public void ParrallelTest()
        {
            DataRequester re = new DataRequester();
            re.UpdateParallelly();
        }
        [Test]
        public void NormalTest()
        {
            DataRequester re = new DataRequester();
            re.UpdateAllData();
        }

    }
}