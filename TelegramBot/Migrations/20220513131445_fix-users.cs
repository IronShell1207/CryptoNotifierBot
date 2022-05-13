using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class fixusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers");

            migrationBuilder.AddColumn<int>(
                name: "BanId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BanId",
                table: "Users",
                column: "BanId");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BannedUsers_BanId",
                table: "Users",
                column: "BanId",
                principalTable: "BannedUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_BannedUsers_BanId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BanId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers");

            migrationBuilder.DropColumn(
                name: "BanId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_UserId",
                table: "BannedUsers",
                column: "UserId",
                unique: true);
        }
    }
}
