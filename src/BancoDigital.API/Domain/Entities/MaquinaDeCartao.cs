namespace BancoDigital.API.Domain.Entities;

public class MaquinaDeCartao : Produto
{
    public string Modelo { get; set; } = string.Empty;
    public decimal TaxaMdrDebito { get; set; }
    public decimal TaxaMdrCredito { get; set; }
    public decimal AluguelMensal { get; set; }

    public override string TipoProduto => "MaquinaDeCartao";
}
