using CryptoApi.Static;
using CryptoApi.Static.DataHandler;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects.ExchangesPairs;

namespace CryptoApi.Tests
{
    public class ExchangesDataTester
    {
        [Test]
        public async Task BitgetTest()
        {
            using (BitgetApi API = new BitgetApi())
                await API.GetExchangeData();
            Assert.Pass(await GetBtcPrice(Exchanges.Bitget));
           
        }
        [Test]
        public async Task BinanceTest()
        {
            using (BinanceApi API = new BinanceApi())
                await API.GetExchangeData();
            Assert.Pass( await GetBtcPrice(Exchanges.Binance));
        }
        [Test]
        public async Task OkxTest()
        {
            using (OkxApi API = new OkxApi())
                await API.GetExchangeData();
            Assert.Pass(await GetBtcPrice(Exchanges.Okx));
        }
        [Test]
        public async Task KucoinTest()
        {
            using (KucoinAPI API = new KucoinAPI())
                await API.GetExchangeData();
            Assert.Pass(await GetBtcPrice(Exchanges.Kucoin));
        }
        [Test]
        public async Task GateioTest()
        {
            using (GateioApi API = new GateioApi())
                await API.GetExchangeData();
            Assert.Pass(await GetBtcPrice(Exchanges.GateIO));
        }

        public async Task<string> GetBtcPrice(string exchange)
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var dbset = dbContext.DataSet.OrderBy(x => x.Id).LastOrDefault(x => x.Exchange == exchange);
                var pairs = dbContext.TradingPairs.Where(x => x.DbId == dbset.Id);
                var btc = pairs.First(x => x.Name == "BTC" && x.Quote == "USDT");
                if (btc?.DbId == dbset.Id)
                    return $"{btc.ToString()}: {btc.Price} created in {dbset.GetDateTime().ToString()} from {btc.Exchange}";
                else Assert.Fail(pairs.Count().ToString());
            }

            return "";
        }
    }
}