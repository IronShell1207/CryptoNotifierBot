using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
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
        public DbSet<MonObj> MonPairs { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }
        public DbSet<MessageAccepted> AcceptedMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}telegrambot.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        private bool Migrating = false;

        public async void MigrateStart()
        {
            var migr = await Database.GetPendingMigrationsAsync();
        start:
            if (migr.Any() && !Migrating)
            {
                Migrating = true;

                foreach (var migration in migr.ToList())
                    ConsoleCommandsHandler.LogWrite($"Migration applying: {migration}");
                try
                {
                    await Database.MigrateAsync();
                }
                catch (Microsoft.Data.Sqlite.SqliteException ex)
                {
                    if (ex.ErrorCode == -2147467259)
                        Thread.Sleep(100);
                    goto start;
                }
            }
        }

        public AppDbContext()
        {
           
            //remove this for create migrations
        }
    }
}