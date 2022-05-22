using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApiPortal
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            var migr = Database.GetPendingMigrations();
            if (migr.Any())
            {
                foreach (var migration in migr.ToList())
                    Console.WriteLine($"Migration applying: {migration}");
                Database.Migrate();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/Tcryptobot/";
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}webportalbase.db";
            optionsBuilder.UseSqlite(dbPath);
        }
    }
}
