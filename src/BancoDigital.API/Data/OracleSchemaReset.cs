using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Data;

public static class OracleSchemaReset
{
    private static readonly string[] TabelasGerenciadas =
    {
        "CONTRATACOES",
        "CLIENTES",
        "PRODUTOS",
        "AGENCIAS",
        "__EFMigrationsHistory"
    };

    public static async Task DropTabelasGerenciadasAsync(AppDbContext db)
    {
        foreach (var tabela in TabelasGerenciadas)
        {
            var sql = $@"BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE ""{tabela}"" CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN
   IF SQLCODE != -942 THEN RAISE; END IF;
END;";
            await db.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
