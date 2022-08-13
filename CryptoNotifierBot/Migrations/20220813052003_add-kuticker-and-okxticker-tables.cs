using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoApi.Migrations
{
    public partial class addkutickerandokxtickertables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Quote",
                table: "TradingPairs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TradingPairs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "KucoinPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    symbol = table.Column<string>(type: "TEXT", nullable: true),
                    symbolName = table.Column<string>(type: "TEXT", nullable: true),
                    buy = table.Column<string>(type: "TEXT", nullable: true),
                    sell = table.Column<string>(type: "TEXT", nullable: true),
                    changeRate = table.Column<string>(type: "TEXT", nullable: true),
                    changePrice = table.Column<string>(type: "TEXT", nullable: true),
                    high = table.Column<string>(type: "TEXT", nullable: true),
                    low = table.Column<string>(type: "TEXT", nullable: true),
                    vol = table.Column<string>(type: "TEXT", nullable: true),
                    volValue = table.Column<string>(type: "TEXT", nullable: true),
                    last = table.Column<string>(type: "TEXT", nullable: true),
                    averagePrice = table.Column<string>(type: "TEXT", nullable: true),
                    takerCoefficient = table.Column<string>(type: "TEXT", nullable: true),
                    makerCoefficient = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KucoinPairs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OkxPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    instId = table.Column<string>(type: "TEXT", nullable: true),
                    last = table.Column<string>(type: "TEXT", nullable: true),
                    lastSz = table.Column<string>(type: "TEXT", nullable: true),
                    askPx = table.Column<string>(type: "TEXT", nullable: true),
                    askSz = table.Column<string>(type: "TEXT", nullable: true),
                    bidPx = table.Column<string>(type: "TEXT", nullable: true),
                    bidSz = table.Column<string>(type: "TEXT", nullable: true),
                    open24h = table.Column<string>(type: "TEXT", nullable: true),
                    high24h = table.Column<string>(type: "TEXT", nullable: true),
                    low24h = table.Column<string>(type: "TEXT", nullable: true),
                    volCcy24h = table.Column<string>(type: "TEXT", nullable: true),
                    vol24h = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OkxPairs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KucoinPairs");

            migrationBuilder.DropTable(
                name: "OkxPairs");

            migrationBuilder.AlterColumn<string>(
                name: "Quote",
                table: "TradingPairs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TradingPairs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
