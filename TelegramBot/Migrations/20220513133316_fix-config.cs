using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class fixconfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreakoutSubs_Users_UserId",
                table: "BreakoutSubs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BreakoutSubs",
                newName: "UserConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_BreakoutSubs_UserId",
                table: "BreakoutSubs",
                newName: "IX_BreakoutSubs_UserConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_BreakoutSubs_Users_UserConfigId",
                table: "BreakoutSubs",
                column: "UserConfigId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreakoutSubs_Users_UserConfigId",
                table: "BreakoutSubs");

            migrationBuilder.RenameColumn(
                name: "UserConfigId",
                table: "BreakoutSubs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BreakoutSubs_UserConfigId",
                table: "BreakoutSubs",
                newName: "IX_BreakoutSubs_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BreakoutSubs_Users_UserId",
                table: "BreakoutSubs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
