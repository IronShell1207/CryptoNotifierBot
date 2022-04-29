using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class addedrelationswithentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BreakoutSubs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoPairs_OwnerId",
                table: "CryptoPairs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BreakoutSubs_UserId",
                table: "BreakoutSubs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackListedPairs_OwnerId",
                table: "BlackListedPairs",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlackListedPairs_BreakoutSubs_OwnerId",
                table: "BlackListedPairs",
                column: "OwnerId",
                principalTable: "BreakoutSubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BreakoutSubs_Users_UserId",
                table: "BreakoutSubs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoPairs_Users_OwnerId",
                table: "CryptoPairs",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlackListedPairs_BreakoutSubs_OwnerId",
                table: "BlackListedPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_BreakoutSubs_Users_UserId",
                table: "BreakoutSubs");

            migrationBuilder.DropForeignKey(
                name: "FK_CryptoPairs_Users_OwnerId",
                table: "CryptoPairs");

            migrationBuilder.DropIndex(
                name: "IX_CryptoPairs_OwnerId",
                table: "CryptoPairs");

            migrationBuilder.DropIndex(
                name: "IX_BreakoutSubs_UserId",
                table: "BreakoutSubs");

            migrationBuilder.DropIndex(
                name: "IX_BlackListedPairs_OwnerId",
                table: "BlackListedPairs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BreakoutSubs");
        }
    }
}
