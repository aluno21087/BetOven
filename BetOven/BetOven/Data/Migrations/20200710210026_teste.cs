using Microsoft.EntityFrameworkCore.Migrations;

namespace BetOven.Data.Migrations
{
    public partial class teste : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotografiaA",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "FotografiaB",
                table: "Jogos");

            migrationBuilder.AddColumn<string>(
                name: "FotoA",
                table: "Jogos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoB",
                table: "Jogos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoA",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "FotoB",
                table: "Jogos");

            migrationBuilder.AddColumn<string>(
                name: "FotografiaA",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotografiaB",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
