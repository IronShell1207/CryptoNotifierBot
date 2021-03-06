using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore;

namespace CryptoApi.Static.DataHandler
{
    public class DataBaseContext : DbContext
    {
        public DbSet<CryDbSet> DataSet { get; set; }
        public DbSet<PricedTradingPair> TradingPairs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            string dbPath = @"D:\Programs\Tbase\"; //Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}cryptodata.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        private bool Migrating = false;
        public DataBaseContext()
        {
            //remove this for create migrations
            var migr = Database.GetPendingMigrations();
            if (migr.Any() && !Migrating)
            {
                Migrating = true;
                try
                {
                    Database.Migrate();
                }
                catch (Microsoft.Data.Sqlite.SqliteException ex)
                {
                    if (ex.ErrorCode == -2147467259)
                        Thread.Sleep(100);

                }
                foreach (var migration in migr.ToList())
                    Diff.LogWrite($"Migration applying: {migration}");
            }

        }

    }
}
