using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class removedrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreakoutSubs_Users_UserConfigId",
                table: "BreakoutSubs");

            migrationBuilder.DropIndex(
                name: "IX_BreakoutSubs_UserConfigId",
                table: "BreakoutSubs");

            migrationBuilder.RenameColumn(
                name: "UserConfigId",
                table: "BreakoutSubs",
                newName: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BreakoutSubs",
                newName: "UserConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BreakoutSubs_UserConfigId",
                table: "BreakoutSubs",
                column: "UserConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_BreakoutSubs_Users_UserConfigId",
                table: "BreakoutSubs",
                column: "UserConfigId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
