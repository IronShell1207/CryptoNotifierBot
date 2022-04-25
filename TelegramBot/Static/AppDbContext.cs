﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects.ExchangesPairs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserConfig> Users { get; set; }
        public DbSet<CryptoPair> CryptoPairs { get; set; }
        public DbSet<BlackListedPairs> BlackListedPairs { get; set; }
        public DbSet<BreakoutSub> BreakoutSubs { get; set; } 
        public DbSet<NotifyMyPos> PositionsNotify { get; set; }
        public DbSet<PositionPair> Positions { get; set; }
        public DbSet<Takes> PositionTakes { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}telegrambot.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        public AppDbContext()
        {
            var migr=  Database.GetPendingMigrations();
            var appl = Database.GetAppliedMigrations();
            if (migr.Any())
            {
                Console.WriteLine("Migration");
                Database.Migrate();
            }
        }
    }
}
