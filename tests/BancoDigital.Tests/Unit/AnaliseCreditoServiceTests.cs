using BancoDigital.API.Domain.Entities;
using BancoDigital.API.Services;
using FluentAssertions;

namespace BancoDigital.Tests.Unit;

public class AnaliseCreditoServiceTests
{
    private readonly AnaliseCreditoService _svc = new();

    private static Emprestimo Produto() => new()
    {
        Nome = "Empréstimo",
        ValorMinimo = 500m,
        ValorMaximo = 100000m,
        PrazoMaximoMeses = 60,
        TaxaJurosBaseAoMes = 0.025m
    };

    [Fact]
    public void PF_com_renda_alta_e_idade_adequada_deve_ser_aprovada()
    {
        var pf = new PessoaFisica
        {
            Nome = "Cliente",
            DataNascimento = DateTime.UtcNow.AddYears(-35),
            RendaMensal = 12000m
        };

        var result = _svc.Analisar(pf, Produto(), 5000m, 24);

        result.Aprovado.Should().BeTrue();
        result.Score.Should().BeGreaterThan(500);
        result.TaxaJurosAplicada.Should().BeLessOrEqualTo(0.025m);
    }

    [Fact]
    public void PF_solicitando_valor_acima_do_maximo_deve_ser_reprovada()
    {
        var pf = new PessoaFisica
        {
            Nome = "Cliente",
            DataNascimento = DateTime.UtcNow.AddYears(-30),
            RendaMensal = 8000m
        };

        var result = _svc.Analisar(pf, Produto(), 200000m, 24);

        result.Aprovado.Should().BeFalse();
        result.Motivo.Should().Contain("máximo");
    }

    [Fact]
    public void PF_com_renda_muito_baixa_e_valor_alto_deve_ter_score_reduzido()
    {
        var pf = new PessoaFisica
        {
            Nome = "Cliente",
            DataNascimento = DateTime.UtcNow.AddYears(-25),
            RendaMensal = 1000m
        };

        var result = _svc.Analisar(pf, Produto(), 50000m, 36);

        result.Score.Should().BeLessThan(400);
    }

    [Fact]
    public void PJ_consolidada_com_alto_faturamento_deve_ser_aprovada()
    {
        var pj = new PessoaJuridica
        {
            Nome = "Empresa",
            RazaoSocial = "Empresa LTDA",
            DataAbertura = DateTime.UtcNow.AddYears(-8),
            FaturamentoMensal = 200000m
        };

        var result = _svc.Analisar(pj, Produto(), 50000m, 24);

        result.Aprovado.Should().BeTrue();
        result.Score.Should().BeGreaterThan(700);
    }

    [Fact]
    public void Score_alto_deve_aplicar_taxa_menor_que_taxa_base()
    {
        var pf = new PessoaFisica
        {
            Nome = "Cliente Premium",
            DataNascimento = DateTime.UtcNow.AddYears(-40),
            RendaMensal = 25000m
        };

        var result = _svc.Analisar(pf, Produto(), 5000m, 24);

        result.Aprovado.Should().BeTrue();
        result.TaxaJurosAplicada.Should().BeLessThan(Produto().TaxaJurosBaseAoMes);
    }
}

public class DocumentoValidatorTests
{
    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("111.444.777-35", true)]
    [InlineData("111.111.111-11", false)]
    [InlineData("123.456.789-00", false)]
    [InlineData("", false)]
    public void CpfValido_avalia_corretamente(string cpf, bool esperado)
        => DocumentoValidator.CpfValido(cpf).Should().Be(esperado);

    [Theory]
    [InlineData("11.444.777/0001-61", true)]
    [InlineData("45.997.418/0001-53", true)]
    [InlineData("00.000.000/0000-00", false)]
    [InlineData("11.111.111/1111-11", false)]
    public void CnpjValido_avalia_corretamente(string cnpj, bool esperado)
        => DocumentoValidator.CnpjValido(cnpj).Should().Be(esperado);
}
