using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BreakoutSubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TelegramId = table.Column<long>(type: "INTEGER", nullable: false),
                    Subscribed = table.Column<bool>(type: "INTEGER", nullable: false),
                    GateioSub = table.Column<bool>(type: "INTEGER", nullable: false),
                    BinanceSub = table.Column<bool>(type: "INTEGER", nullable: false),
                    KucoinSub = table.Column<bool>(type: "INTEGER", nullable: false),
                    OkxSub = table.Column<bool>(type: "INTEGER", nullable: false),
                    S5MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S2MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S15MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S30MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S45MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S60MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S120MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S240MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S480MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S960MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    S1920MinUpdates = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreakoutSubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CryptoPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PairBase = table.Column<string>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    PairQuote = table.Column<string>(type: "TEXT", nullable: false),
                    ExchangePlatform = table.Column<string>(type: "TEXT", nullable: false),
                    GainOrFall = table.Column<bool>(type: "INTEGER", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    TriggerOnce = table.Column<bool>(type: "INTEGER", nullable: false),
                    Screenshot = table.Column<string>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoPairs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoritePairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Base = table.Column<string>(type: "TEXT", nullable: false),
                    Quote = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePairs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TelegramId = table.Column<long>(type: "INTEGER", nullable: false),
                    NoticationsInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    AntifloodIntervalAmplification = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotesEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CryptoNotifyStyle = table.Column<bool>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayTaskEditButtonsInNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    NightModeEnable = table.Column<bool>(type: "INTEGER", nullable: false),
                    NightModeStartTime = table.Column<int>(type: "INTEGER", nullable: false),
                    NightModeEndsTime = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowMarketEvents = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BreakoutSubs");

            migrationBuilder.DropTable(
                name: "CryptoPairs");

            migrationBuilder.DropTable(
                name: "FavoritePairs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
