using BancoDigital.API.Domain.Enums;

namespace BancoDigital.API.Domain.Entities;

public class Contratacao
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public decimal ValorSolicitado { get; set; }
    public int? PrazoMeses { get; set; }

    public StatusContratacao Status { get; set; } = StatusContratacao.Pendente;
    public string? MotivoStatus { get; set; }

    public int? ScoreCalculado { get; set; }
    public decimal? TaxaJurosAplicada { get; set; }
    public decimal? ValorAprovado { get; set; }

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataProcessamento { get; set; }
}
