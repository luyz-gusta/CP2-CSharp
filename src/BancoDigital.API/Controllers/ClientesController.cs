using BancoDigital.API.Data;
using BancoDigital.API.Domain.Entities;
using BancoDigital.API.DTOs;
using BancoDigital.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClientesController(AppDbContext db) => _db = db;

    [HttpPost("pf")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CriarPf([FromBody] CriarPessoaFisicaRequest req)
    {
        var cpf = DocumentoValidator.LimparCpf(req.Cpf);
        if (!DocumentoValidator.CpfValido(cpf))
            return BadRequest(new { mensagem = "CPF inválido." });

        if (req.DataNascimento >= DateTime.UtcNow.Date)
            return BadRequest(new { mensagem = "Data de nascimento inválida." });

        var agencia = await _db.Agencias.FindAsync(req.AgenciaId);
        if (agencia is null)
            return NotFound(new { mensagem = $"Agência {req.AgenciaId} não encontrada." });

        if (await _db.PessoasFisicas.Where(p => p.Cpf == cpf).CountAsync() > 0)
            return Conflict(new { mensagem = "Já existe pessoa física cadastrada com este CPF." });

        var pf = new PessoaFisica
        {
            Nome = req.Nome,
            Email = req.Email,
            Telefone = req.Telefone ?? string.Empty,
            Cpf = cpf,
            DataNascimento = DateTime.SpecifyKind(req.DataNascimento.Date, DateTimeKind.Utc),
            RendaMensal = req.RendaMensal,
            AgenciaId = req.AgenciaId
        };
        _db.PessoasFisicas.Add(pf);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = pf.Id }, MapearCliente(pf, agencia));
    }

    [HttpPost("pj")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CriarPj([FromBody] CriarPessoaJuridicaRequest req)
    {
        var cnpj = DocumentoValidator.LimparCnpj(req.Cnpj);
        if (!DocumentoValidator.CnpjValido(cnpj))
            return BadRequest(new { mensagem = "CNPJ inválido." });

        if (req.DataAbertura >= DateTime.UtcNow.Date)
            return BadRequest(new { mensagem = "Data de abertura inválida." });

        var agencia = await _db.Agencias.FindAsync(req.AgenciaId);
        if (agencia is null)
            return NotFound(new { mensagem = $"Agência {req.AgenciaId} não encontrada." });

        if (await _db.PessoasJuridicas.Where(p => p.Cnpj == cnpj).CountAsync() > 0)
            return Conflict(new { mensagem = "Já existe pessoa jurídica cadastrada com este CNPJ." });

        var pj = new PessoaJuridica
        {
            Nome = req.Nome,
            Email = req.Email,
            Telefone = req.Telefone ?? string.Empty,
            Cnpj = cnpj,
            RazaoSocial = req.RazaoSocial,
            DataAbertura = DateTime.SpecifyKind(req.DataAbertura.Date, DateTimeKind.Utc),
            FaturamentoMensal = req.FaturamentoMensal,
            AgenciaId = req.AgenciaId
        };
        _db.PessoasJuridicas.Add(pj);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = pj.Id }, MapearCliente(pj, agencia));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var cliente = await _db.Clientes
            .Include(c => c.Agencia)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        return Ok(MapearCliente(cliente, cliente.Agencia));
    }

    private static ClienteResponse MapearCliente(Cliente c, Agencia? agencia) => c switch
    {
        PessoaFisica pf => new ClienteResponse(
            pf.Id, "PF", pf.Nome, pf.Email, pf.Telefone, pf.Cpf,
            pf.AgenciaId, agencia?.Nome, pf.DataCadastro,
            pf.DataNascimento, pf.RendaMensal,
            null, null, null),
        PessoaJuridica pj => new ClienteResponse(
            pj.Id, "PJ", pj.Nome, pj.Email, pj.Telefone, pj.Cnpj,
            pj.AgenciaId, agencia?.Nome, pj.DataCadastro,
            null, null,
            pj.RazaoSocial, pj.DataAbertura, pj.FaturamentoMensal),
        _ => throw new InvalidOperationException("Tipo de cliente desconhecido.")
    };
}
