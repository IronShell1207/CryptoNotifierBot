using CryptoWebPortal.Objects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWebPortal.Static
{
    public class WebDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}webdb.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        public WebDbContext()
        {
            var migration = Database.GetPendingMigrations();
            if (migration.Any())
                Database.Migrate();
        }

    }
}
