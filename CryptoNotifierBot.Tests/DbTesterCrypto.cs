using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.API;
using CryptoApi.Constants;
using CryptoApi.Objects;
using CryptoApi.Static.DataHandler;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore.Sqlite;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoNotifierBot.Tests
{
    public class DbTesterCrypto
    {
        [Test]
        public void DeletingTest()
        {
            using (DataBaseContext dbContext = new DataBaseContext())
            { 
                dbContext.Database.ExecuteSqlRaw(
                    "DELETE FROM DataSet WHERE Id in (SELECT Id FROM DataSet ORDER BY Id LIMIT 2)");
                

            }
        }

        [Test]
        public async Task CMCTest()
        {

            using (CoinMarketCapTop cmctop = new CoinMarketCapTop())
            {
                var data= await cmctop.Get(200);
                var btc = data.First(x => x.symbol == "BTC");
                if (btc!=null) Assert.Pass($"{btc.symbol}: {btc.circulating_supply} {btc.date_added}");
                else Assert.Fail($"{btc.ToString()}");
            }

           


        }
    }
}
