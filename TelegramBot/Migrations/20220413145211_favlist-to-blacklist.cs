using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class favlisttoblacklist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OnlyFavList",
                table: "BreakoutSubs",
                newName: "BlackListEnable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlackListEnable",
                table: "BreakoutSubs",
                newName: "OnlyFavList");
        }
    }
}
