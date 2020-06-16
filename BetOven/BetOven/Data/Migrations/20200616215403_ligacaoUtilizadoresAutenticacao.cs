using Microsoft.EntityFrameworkCore.Migrations;

namespace BetOven.Data.Migrations
{
    public partial class ligacaoUtilizadoresAutenticacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsernameID",
                table: "Utilizadores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsernameID",
                table: "Utilizadores");
        }
    }
}
