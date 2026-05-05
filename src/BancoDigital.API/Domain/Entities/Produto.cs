namespace BancoDigital.API.Domain.Entities;

public abstract class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public abstract string TipoProduto { get; }
}
