using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
