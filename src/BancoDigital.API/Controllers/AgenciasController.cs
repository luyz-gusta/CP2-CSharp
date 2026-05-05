using BancoDigital.API.Data;
using BancoDigital.API.Domain.Entities;
using BancoDigital.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/agencias")]
public class AgenciasController : ControllerBase
{
    private readonly AppDbContext _db;

    public AgenciasController(AppDbContext db) => _db = db;

    [HttpPost]
    [ProducesResponseType(typeof(AgenciaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] CriarAgenciaRequest req)
    {
        if (await _db.Agencias.Where(a => a.Codigo == req.Codigo).CountAsync() > 0)
            return Conflict(new { mensagem = $"Já existe agência com código {req.Codigo}." });

        var agencia = new Agencia
        {
            Nome = req.Nome,
            Codigo = req.Codigo,
            Endereco = req.Endereco ?? string.Empty
        };
        _db.Agencias.Add(agencia);
        await _db.SaveChangesAsync();

        var resp = new AgenciaResponse(agencia.Id, agencia.Nome, agencia.Codigo, agencia.Endereco);
        return CreatedAtAction(nameof(BuscarPorId), new { id = agencia.Id }, resp);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AgenciaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var ag = await _db.Agencias.FindAsync(id);
        if (ag is null) return NotFound(new { mensagem = "Agência não encontrada." });
        return Ok(new AgenciaResponse(ag.Id, ag.Nome, ag.Codigo, ag.Endereco));
    }
}
