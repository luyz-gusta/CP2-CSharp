using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDigital.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AGENCIAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Codigo = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AGENCIAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Ativo = table.Column<int>(type: "NUMBER(1)", nullable: false),
                    TIPO_PRODUTO = table.Column<string>(type: "NVARCHAR2(21)", maxLength: 21, nullable: false),
                    ValorMinimo = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true),
                    ValorMaximo = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true),
                    PrazoMaximoMeses = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TaxaJurosBaseAoMes = table.Column<decimal>(type: "NUMBER(9,4)", nullable: true),
                    Modelo = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    TaxaMdrDebito = table.Column<decimal>(type: "NUMBER(9,4)", nullable: true),
                    TaxaMdrCredito = table.Column<decimal>(type: "NUMBER(9,4)", nullable: true),
                    AluguelMensal = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true),
                    EmpresaConveniada = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CnpjEmpregador = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CLIENTES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AgenciaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TIPO_CLIENTE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Cpf = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    RendaMensal = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true),
                    Cnpj = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: true),
                    RazaoSocial = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    DataAbertura = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    FaturamentoMensal = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENTES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CLIENTES_AGENCIAS_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "AGENCIAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONTRATACOES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClienteId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProdutoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ValorSolicitado = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    PrazoMeses = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MotivoStatus = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ScoreCalculado = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TaxaJurosAplicada = table.Column<decimal>(type: "NUMBER(9,4)", nullable: true),
                    ValorAprovado = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true),
                    DataSolicitacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRATACOES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CONTRATACOES_CLIENTES_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "CLIENTES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONTRATACOES_PRODUTOS_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AGENCIAS_Codigo",
                table: "AGENCIAS",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTES_AgenciaId",
                table: "CLIENTES",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTES_Cnpj",
                table: "CLIENTES",
                column: "Cnpj",
                unique: true,
                filter: "\"Cnpj\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTES_Cpf",
                table: "CLIENTES",
                column: "Cpf",
                unique: true,
                filter: "\"Cpf\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATACOES_ClienteId",
                table: "CONTRATACOES",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATACOES_ProdutoId",
                table: "CONTRATACOES",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONTRATACOES");

            migrationBuilder.DropTable(
                name: "CLIENTES");

            migrationBuilder.DropTable(
                name: "PRODUTOS");

            migrationBuilder.DropTable(
                name: "AGENCIAS");
        }
    }
}
