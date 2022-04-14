using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserConfig> Users { get; set; }
        public DbSet<CryptoPair> CryptoPairs { get; set; }
        public DbSet<BlackListedPairs> FavoritePairs { get; set; }
        public DbSet<BreakoutSub> BreakoutSubs { get; set; } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}telegrambot.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }
    }
}
