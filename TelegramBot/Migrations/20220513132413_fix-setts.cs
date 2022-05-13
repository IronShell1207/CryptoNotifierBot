using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class fixsetts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_BannedUsers_BanId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_BreakoutSubs_BreakoutSubId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BanId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BreakoutSubId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BanId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BreakoutSubId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BreakoutSubs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BreakoutSubs_UserId",
                table: "BreakoutSubs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BreakoutSubs_Users_UserId",
                table: "BreakoutSubs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreakoutSubs_Users_UserId",
                table: "BreakoutSubs");

            migrationBuilder.DropIndex(
                name: "IX_BreakoutSubs_UserId",
                table: "BreakoutSubs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BreakoutSubs");

            migrationBuilder.AddColumn<int>(
                name: "BanId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakoutSubId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BanId",
                table: "Users",
                column: "BanId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BreakoutSubId",
                table: "Users",
                column: "BreakoutSubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BannedUsers_BanId",
                table: "Users",
                column: "BanId",
                principalTable: "BannedUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BreakoutSubs_BreakoutSubId",
                table: "Users",
                column: "BreakoutSubId",
                principalTable: "BreakoutSubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
