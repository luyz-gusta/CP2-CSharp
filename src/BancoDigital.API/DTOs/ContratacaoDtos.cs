using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.DTOs;

public record CriarContratacaoRequest(
    [Required] int ClienteId,
    [Required] int ProdutoId,
    [Range(0.01, 99999999)] decimal ValorSolicitado,
    [Range(1, 360)] int? PrazoMeses
);

public record ContratacaoResponse(
    int Id,
    int ClienteId,
    int ProdutoId,
    decimal ValorSolicitado,
    int? PrazoMeses,
    string Status,
    string? MotivoStatus,
    int? ScoreCalculado,
    decimal? TaxaJurosAplicada,
    decimal? ValorAprovado,
    DateTime DataSolicitacao,
    DateTime? DataProcessamento
);

public record ContratacaoMensagem(int ContratacaoId);
