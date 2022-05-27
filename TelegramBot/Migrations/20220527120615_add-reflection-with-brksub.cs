using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class addreflectionwithbrksub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubSetsId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SubSetsId",
                table: "Users",
                column: "SubSetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BreakoutSubs_SubSetsId",
                table: "Users",
                column: "SubSetsId",
                principalTable: "BreakoutSubs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_BreakoutSubs_SubSetsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SubSetsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubSetsId",
                table: "Users");
        }
    }
}
