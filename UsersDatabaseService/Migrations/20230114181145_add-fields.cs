using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersDatabaseService.Migrations
{
    /// <inheritdoc />
    public partial class addfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AntiFloodIntervalAmplification",
                table: "TelegramUserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoSetExchange",
                table: "TelegramUserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "TelegramUserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MorningReport",
                table: "TelegramUserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NightModeEnable",
                table: "TelegramUserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "NightModeEndsTime",
                table: "TelegramUserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "NightModeStartTime",
                table: "TelegramUserSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "NoticationsInterval",
                table: "TelegramUserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowChangesOnce",
                table: "TelegramUserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Timezone",
                table: "TelegramUserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MonitoringPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GainOrFall = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTriggered = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTriggerOnce = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotifyEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    TriggerPrice = table.Column<double>(type: "REAL", nullable: false),
                    CurrencyName = table.Column<string>(type: "TEXT", nullable: false),
                    CurrencyQuote = table.Column<string>(type: "TEXT", nullable: false),
                    ExchangePlatform = table.Column<string>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringPairs_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringPairs_OwnerId",
                table: "MonitoringPairs",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitoringPairs");

            migrationBuilder.DropColumn(
                name: "AntiFloodIntervalAmplification",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "AutoSetExchange",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "MorningReport",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "NightModeEnable",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "NightModeEndsTime",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "NightModeStartTime",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "NoticationsInterval",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "ShowChangesOnce",
                table: "TelegramUserSettings");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "TelegramUserSettings");
        }
    }
}
