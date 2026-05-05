using BancoDigital.API.Domain.Enums;

namespace BancoDigital.API.Domain.Entities;

public abstract class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public int AgenciaId { get; set; }
    public Agencia? Agencia { get; set; }

    public ICollection<Contratacao> Contratacoes { get; set; } = new List<Contratacao>();

    public abstract TipoCliente Tipo { get; }
    public abstract string Documento { get; }
}
