using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoApi.Migrations
{
    public partial class Changeddataconnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DbId",
                table: "TradingPairs",
                newName: "CryDbSetId");

            migrationBuilder.CreateIndex(
                name: "IX_TradingPairs_CryDbSetId",
                table: "TradingPairs",
                column: "CryDbSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_TradingPairs_DataSet_CryDbSetId",
                table: "TradingPairs",
                column: "CryDbSetId",
                principalTable: "DataSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradingPairs_DataSet_CryDbSetId",
                table: "TradingPairs");

            migrationBuilder.DropIndex(
                name: "IX_TradingPairs_CryDbSetId",
                table: "TradingPairs");

            migrationBuilder.RenameColumn(
                name: "CryDbSetId",
                table: "TradingPairs",
                newName: "DbId");
        }
    }
}
