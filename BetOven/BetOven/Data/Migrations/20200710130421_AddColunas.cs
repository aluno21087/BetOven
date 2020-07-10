using Microsoft.EntityFrameworkCore.Migrations;

namespace BetOven.Data.Migrations
{
    public partial class AddColunas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotografiaA",
                table: "Jogos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotografiaB",
                table: "Jogos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
