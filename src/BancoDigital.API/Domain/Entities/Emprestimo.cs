namespace BancoDigital.API.Domain.Entities;

public class Emprestimo : Produto
{
    public decimal ValorMinimo { get; set; }
    public decimal ValorMaximo { get; set; }
    public int PrazoMaximoMeses { get; set; }
    public decimal TaxaJurosBaseAoMes { get; set; }

    public override string TipoProduto => "Emprestimo";
}
