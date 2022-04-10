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
        public DbSet<FavotitePairs> FavoritePairs { get; set; }
        public DbSet<BreakoutSub> BreakoutSubs { get; set; } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(AppSettingsStatic.Settings.DbConnectionString);
            // base.OnConfiguring(optionsBuilder);
        }

        public AppDbContext()
        {
            //Database.Migrate();
            Database.EnsureCreated();
        }
    }
}
