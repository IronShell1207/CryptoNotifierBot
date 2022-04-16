using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class blacklistAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoritePairs",
                table: "FavoritePairs");

            migrationBuilder.RenameTable(
                name: "FavoritePairs",
                newName: "BlackListedPairs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlackListedPairs",
                table: "BlackListedPairs",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlackListedPairs",
                table: "BlackListedPairs");

            migrationBuilder.RenameTable(
                name: "BlackListedPairs",
                newName: "FavoritePairs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoritePairs",
                table: "FavoritePairs",
                column: "Id");
        }
    }
}
