using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.DTOs;

public record CriarAgenciaRequest(
    [Required, StringLength(120)] string Nome,
    [Required, StringLength(10)] string Codigo,
    [StringLength(200)] string? Endereco
);

public record AgenciaResponse(int Id, string Nome, string Codigo, string? Endereco);
