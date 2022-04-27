using CryptoApi.Static;
using CryptoApi.Static.DataHandler;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoApi.Tests
{
    public class ExchangesDataTester
    {
        [Test]
        public async Task BitgetTest()
        {
            using (BitgetApi API = new BitgetApi())
            {
                await API.GetExchangeData();
            }

            using (DataBaseContext dbContext = new DataBaseContext())
            {
                var dbset = dbContext.DataSet.OrderBy(x=>x.Id).LastOrDefault();
                var pairs = dbContext.TradingPairs.Where(x => x.DbId == dbset.Id);
                var btc = pairs.First(x => x.Name == "BTC" && x.Quote=="USDT");
                if (btc?.DbId == dbset.Id)
                    Assert.Pass($"{btc.ToString()}: {btc.Price} created in {dbset.GetDateTime().ToString()} from {btc.Exchange}");
                else Assert.Fail(pairs.Count().ToString());
            }
        }
    }
}