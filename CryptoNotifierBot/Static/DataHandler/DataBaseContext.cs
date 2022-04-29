﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}cryptodata.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        public DataBaseContext()
        {
            //remove this for create migrations
#if DEBUG
            var migr = Database.GetPendingMigrations();
            var appl = Database.GetAppliedMigrations();
            if (migr.Any())
            {
                Console.WriteLine("Migration");
                Database.Migrate();
            }
#endif
        }
        
    }
}