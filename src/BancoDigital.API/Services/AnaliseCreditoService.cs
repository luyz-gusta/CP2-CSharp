using BancoDigital.API.Domain.Entities;

namespace BancoDigital.API.Services;

public record ResultadoAnalise(
    int Score,
    bool Aprovado,
    decimal TaxaJurosAplicada,
    decimal ValorAprovado,
    string Motivo
);

public class AnaliseCreditoService
{
    public ResultadoAnalise Analisar(Cliente cliente, Emprestimo produto, decimal valorSolicitado, int? prazoMeses)
    {
        var score = CalcularScore(cliente, valorSolicitado);
        var (aprovado, motivo) = AvaliarAprovacao(score, valorSolicitado, produto);

        if (!aprovado)
            return new ResultadoAnalise(score, false, 0m, 0m, motivo);

        var taxaAplicada = CalcularTaxa(produto.TaxaJurosBaseAoMes, score);
        var valorAprovado = AjustarValorAprovado(valorSolicitado, score, produto);

        return new ResultadoAnalise(score, true, taxaAplicada, valorAprovado,
            $"Aprovado com score {score}. Taxa aplicada: {taxaAplicada:P2} a.m.");
    }

    private static int CalcularScore(Cliente cliente, decimal valorSolicitado)
    {
        var score = 500;

        if (cliente is PessoaFisica pf)
        {
            var idade = pf.CalcularIdade();
            if (idade < 18) return 0;
            if (idade is >= 25 and <= 60) score += 100;
            else if (idade > 60) score += 50;

            if (pf.RendaMensal >= 10000) score += 250;
            else if (pf.RendaMensal >= 5000) score += 150;
            else if (pf.RendaMensal >= 2000) score += 80;
            else if (pf.RendaMensal < 1500) score -= 100;

            if (pf.RendaMensal > 0)
            {
                var razao = valorSolicitado / pf.RendaMensal;
                if (razao > 30) score -= 200;
                else if (razao > 15) score -= 80;
            }
        }
        else if (cliente is PessoaJuridica pj)
        {
            var anos = pj.TempoExistenciaAnos();
            if (anos >= 5) score += 200;
            else if (anos >= 2) score += 100;
            else score -= 50;

            if (pj.FaturamentoMensal >= 100000) score += 200;
            else if (pj.FaturamentoMensal >= 30000) score += 100;
            else if (pj.FaturamentoMensal < 10000) score -= 80;

            if (pj.FaturamentoMensal > 0)
            {
                var razao = valorSolicitado / pj.FaturamentoMensal;
                if (razao > 10) score -= 150;
            }
        }

        return Math.Clamp(score, 0, 1000);
    }

    private static (bool aprovado, string motivo) AvaliarAprovacao(int score, decimal valorSolicitado, Emprestimo produto)
    {
        if (valorSolicitado < produto.ValorMinimo)
            return (false, $"Valor abaixo do mínimo permitido (R$ {produto.ValorMinimo:N2}).");
        if (valorSolicitado > produto.ValorMaximo)
            return (false, $"Valor acima do máximo permitido (R$ {produto.ValorMaximo:N2}).");
        if (score < 300)
            return (false, $"Score insuficiente para aprovação (score={score}).");
        return (true, string.Empty);
    }

    private static decimal CalcularTaxa(decimal taxaBase, int score)
    {
        var ajuste = score switch
        {
            >= 800 => -0.40m,
            >= 650 => -0.20m,
            >= 500 => 0m,
            >= 400 => 0.30m,
            _ => 0.60m
        };
        var taxa = taxaBase * (1 + ajuste);
        return Math.Round(Math.Max(taxa, 0.005m), 4);
    }

    private static decimal AjustarValorAprovado(decimal valorSolicitado, int score, Emprestimo produto)
    {
        var fator = score switch
        {
            >= 700 => 1.0m,
            >= 500 => 0.85m,
            _ => 0.6m
        };
        var valor = Math.Round(valorSolicitado * fator, 2);
        return Math.Clamp(valor, produto.ValorMinimo, produto.ValorMaximo);
    }
}
