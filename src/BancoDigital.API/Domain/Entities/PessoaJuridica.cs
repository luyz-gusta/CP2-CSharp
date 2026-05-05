using BancoDigital.API.Domain.Enums;

namespace BancoDigital.API.Domain.Entities;

public class PessoaJuridica : Cliente
{
    public string Cnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public decimal FaturamentoMensal { get; set; }

    public override TipoCliente Tipo => TipoCliente.PJ;
    public override string Documento => Cnpj;

    public int TempoExistenciaAnos()
    {
        var hoje = DateTime.UtcNow.Date;
        var anos = hoje.Year - DataAbertura.Year;
        if (DataAbertura.Date > hoje.AddYears(-anos)) anos--;
        return anos;
    }
}
