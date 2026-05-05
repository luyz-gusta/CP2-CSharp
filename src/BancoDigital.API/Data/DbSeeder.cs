using BancoDigital.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Produtos.CountAsync() == 0)
        {
            db.Emprestimos.Add(new Emprestimo
            {
                Nome = "Empréstimo Pessoal",
                Descricao = "Empréstimo com taxa variável conforme score do cliente.",
                Ativo = true,
                ValorMinimo = 500m,
                ValorMaximo = 100000m,
                PrazoMaximoMeses = 60,
                TaxaJurosBaseAoMes = 0.025m
            });

            db.MaquinasDeCartao.Add(new MaquinaDeCartao
            {
                Nome = "Maquininha Smart",
                Descricao = "Máquina de cartão com aluguel mensal.",
                Ativo = true,
                Modelo = "Smart-X1",
                TaxaMdrDebito = 0.015m,
                TaxaMdrCredito = 0.029m,
                AluguelMensal = 49.90m
            });

            db.ReceberSalarios.Add(new ReceberSalario
            {
                Nome = "Conta Salário",
                Descricao = "Convênio para recebimento de salário.",
                Ativo = true,
                EmpresaConveniada = "FIAP S.A.",
                CnpjEmpregador = "60704816000172"
            });

            await db.SaveChangesAsync();
        }

        if (await db.Agencias.CountAsync() == 0)
        {
            db.Agencias.Add(new Agencia
            {
                Nome = "Agência Centro",
                Codigo = "0001",
                Endereco = "Av. Paulista, 1000 - São Paulo/SP"
            });
            await db.SaveChangesAsync();
        }
    }
}
