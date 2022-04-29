using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoApi.Migrations
{
    public partial class Separateddateandtimeparametersmerged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "DataSet");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "DataSet",
                newName: "DateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "DataSet",
                newName: "Time");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "DataSet",
                type: "TEXT",
                nullable: true);
        }
    }
}
