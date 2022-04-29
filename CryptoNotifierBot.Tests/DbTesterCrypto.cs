using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Static.DataHandler;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore.Sqlite;

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
    }
}
