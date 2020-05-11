using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BetOven.Migrations
{
    public partial class ConfigInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jogos",
                columns: table => new
                {
                    njogo = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    equipaA = table.Column<string>(nullable: true),
                    equipaB = table.Column<string>(nullable: true),
                    Resultado = table.Column<string>(nullable: true),
                    datainiciojogo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogos", x => x.njogo);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    nickname = table.Column<string>(nullable: true),
                    nacionalidade = table.Column<string>(nullable: true),
                    datanasc = table.Column<DateTime>(nullable: false),
                    saldo = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Apostas",
                columns: table => new
                {
                    nAposta = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantia = table.Column<double>(nullable: false),
                    data = table.Column<DateTime>(nullable: false),
                    estado = table.Column<string>(nullable: true),
                    UserFK = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apostas", x => x.nAposta);
                    table.ForeignKey(
                        name: "FK_Apostas_Users_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    nDeposito = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    montante = table.Column<double>(nullable: false),
                    data = table.Column<DateTime>(nullable: false),
                    formato_pagamento = table.Column<string>(nullable: true),
                    origem_deposito = table.Column<string>(nullable: true),
                    UserFK = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.nDeposito);
                    table.ForeignKey(
                        name: "FK_Depositos_Users_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Apostas_Jogos",
                columns: table => new
                {
                    ApostaFK = table.Column<int>(nullable: false),
                    JogoFK = table.Column<int>(nullable: false),
                    descricao = table.Column<string>(nullable: true),
                    multiplicador = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apostas_Jogos", x => x.ApostaFK);
                    table.ForeignKey(
                        name: "FK_Apostas_Jogos_Apostas_ApostaFK",
                        column: x => x.ApostaFK,
                        principalTable: "Apostas",
                        principalColumn: "nAposta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Apostas_Jogos_Jogos_JogoFK",
                        column: x => x.JogoFK,
                        principalTable: "Jogos",
                        principalColumn: "njogo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_UserFK",
                table: "Apostas",
                column: "UserFK");

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_Jogos_JogoFK",
                table: "Apostas_Jogos",
                column: "JogoFK");

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_UserFK",
                table: "Depositos",
                column: "UserFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apostas_Jogos");

            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropTable(
                name: "Apostas");

            migrationBuilder.DropTable(
                name: "Jogos");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
