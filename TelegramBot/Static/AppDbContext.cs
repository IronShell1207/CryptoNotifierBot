using System;
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
        public DbSet<MessageAccepted> AcceptedMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}telegrambot.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        public AppDbContext()
        {
            //remove this for create migrations
#if DEBUG
            var migr = Database.GetPendingMigrations();

            if (migr.Any())
            {
                foreach (var migration in migr.ToList())
                    ConsoleCommandsHandler.LogWrite($"Migration applying: {migration}");
                Database.Migrate();
            }
#endif
        }
    }
}
