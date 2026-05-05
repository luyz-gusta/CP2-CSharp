using BancoDigital.API.Domain.Enums;

namespace BancoDigital.API.Domain.Entities;

public class PessoaFisica : Cliente
{
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public decimal RendaMensal { get; set; }

    public override TipoCliente Tipo => TipoCliente.PF;
    public override string Documento => Cpf;

    public int CalcularIdade()
    {
        var hoje = DateTime.UtcNow.Date;
        var idade = hoje.Year - DataNascimento.Year;
        if (DataNascimento.Date > hoje.AddYears(-idade)) idade--;
        return idade;
    }
}
