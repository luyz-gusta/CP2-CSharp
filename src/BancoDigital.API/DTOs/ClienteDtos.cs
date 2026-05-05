using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.DTOs;

public record CriarPessoaFisicaRequest(
    [Required, StringLength(150)] string Nome,
    [Required, EmailAddress, StringLength(150)] string Email,
    [StringLength(20)] string? Telefone,
    [Required, StringLength(14, MinimumLength = 11)] string Cpf,
    [Required] DateTime DataNascimento,
    [Range(0, 99999999)] decimal RendaMensal,
    [Required] int AgenciaId
);

public record CriarPessoaJuridicaRequest(
    [Required, StringLength(150)] string Nome,
    [Required, EmailAddress, StringLength(150)] string Email,
    [StringLength(20)] string? Telefone,
    [Required, StringLength(18, MinimumLength = 14)] string Cnpj,
    [Required, StringLength(200)] string RazaoSocial,
    [Required] DateTime DataAbertura,
    [Range(0, 999999999)] decimal FaturamentoMensal,
    [Required] int AgenciaId
);

public record ClienteResponse(
    int Id,
    string Tipo,
    string Nome,
    string Email,
    string? Telefone,
    string Documento,
    int AgenciaId,
    string? AgenciaNome,
    DateTime DataCadastro,
    DateTime? DataNascimento,
    decimal? RendaMensal,
    string? RazaoSocial,
    DateTime? DataAbertura,
    decimal? FaturamentoMensal
);
