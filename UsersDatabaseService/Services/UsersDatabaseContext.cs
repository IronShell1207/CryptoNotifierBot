using Microsoft.EntityFrameworkCore;
using Serilog;
using UsersDatabaseService.Models;

namespace UsersDatabaseService.Services
{
    /// <summary>
    /// База данных.
    /// </summary>
    public class UsersDatabaseContext : DbContext
    {
        #region Private Fields

        private const string DB_NAME = @"\Crybotdata\";

        private bool _isMigrationFailed = false;

        public DbSet<UserModel> Users { get; set; }
        public DbSet<TelegramUserSettings> TelegramUserSettings { get; set; }
        public DbSet<MonitoringPair> MonitoringPairs { get; set; }

        #endregion Private Fields

        #region Public Methods

        public async Task MakeMigrations()
        {
        start:
            var migrations = await Database.GetPendingMigrationsAsync();
            if (migrations.Any())
            {
                Log.Logger.Information($"Pending migrations. Count:{migrations.Count()}");
                foreach (var migration in migrations)
                {
                    Log.Logger.Warning($"Performing migration: {migration}");
                }
                try
                {
                    await Database.MigrateAsync();
                }
                catch (Microsoft.Data.Sqlite.SqliteException ex)
                {
                    Log.Logger.Error(ex, $"An error occurred while migration.");
                    if (!_isMigrationFailed)
                    {
                        if (ex.ErrorCode == -2147467259)
                            Thread.Sleep(150);
                        goto start;
                    }
                    else throw;
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
            //string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/Tcryptobot/";
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + DB_NAME;
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            dbPath = $"Filename={dbPath}usersdb.db";
            optionsBuilder.UseSqlite(dbPath);
        }

        #endregion Protected Methods
    }
}