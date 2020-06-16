using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BetOven.Data.Migrations
{
    public partial class Apostas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jogos",
                columns: table => new
                {
                    Njogo = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipaA = table.Column<string>(nullable: true),
                    EquipaB = table.Column<string>(nullable: true),
                    Resultado = table.Column<string>(nullable: true),
                    Datainiciojogo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogos", x => x.Njogo);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(maxLength: 40, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Nickname = table.Column<string>(maxLength: 20, nullable: false),
                    Nacionalidade = table.Column<string>(nullable: true),
                    Datanasc = table.Column<DateTime>(nullable: false),
                    Saldo = table.Column<double>(nullable: false),
                    Fotografia = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Apostas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantia = table.Column<double>(nullable: false),
                    Data = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(nullable: true),
                    Multiplicador = table.Column<double>(nullable: false),
                    UserFK = table.Column<int>(nullable: false),
                    JogoFK = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apostas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Apostas_Jogos_JogoFK",
                        column: x => x.JogoFK,
                        principalTable: "Jogos",
                        principalColumn: "Njogo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Apostas_Utilizadores_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Utilizadores",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    NDeposito = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Montante = table.Column<double>(nullable: false),
                    Data = table.Column<DateTime>(nullable: false),
                    Formato_pagamento = table.Column<string>(nullable: true),
                    Origem_deposito = table.Column<string>(nullable: true),
                    UserFK = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.NDeposito);
                    table.ForeignKey(
                        name: "FK_Depositos_Utilizadores_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Utilizadores",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_JogoFK",
                table: "Apostas",
                column: "JogoFK");

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_UserFK",
                table: "Apostas",
                column: "UserFK");

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_UserFK",
                table: "Depositos",
                column: "UserFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apostas");

            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropTable(
                name: "Jogos");

            migrationBuilder.DropTable(
                name: "Utilizadores");
        }
    }
}
