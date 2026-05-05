using BancoDigital.API.Data;
using BancoDigital.API.Domain.Entities;
using BancoDigital.API.DTOs;
using BancoDigital.API.Workers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IContratacaoQueue _queue;

    public ContratacoesController(AppDbContext db, IContratacaoQueue queue)
    {
        _db = db;
        _queue = queue;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContratacaoResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Solicitar([FromBody] CriarContratacaoRequest req)
    {
        var cliente = await _db.Clientes.FindAsync(req.ClienteId);
        if (cliente is null)
            return NotFound(new { mensagem = $"Cliente {req.ClienteId} não encontrado." });

        var produto = await _db.Produtos.FindAsync(req.ProdutoId);
        if (produto is null)
            return NotFound(new { mensagem = $"Produto {req.ProdutoId} não encontrado." });

        if (!produto.Ativo)
            return BadRequest(new { mensagem = "Produto inativo." });

        if (produto is Emprestimo emp)
        {
            if (req.PrazoMeses is null)
                return BadRequest(new { mensagem = "PrazoMeses é obrigatório para Empréstimo." });
            if (req.PrazoMeses > emp.PrazoMaximoMeses)
                return BadRequest(new { mensagem = $"Prazo excede o máximo permitido ({emp.PrazoMaximoMeses} meses)." });
        }

        var contratacao = new Contratacao
        {
            ClienteId = req.ClienteId,
            ProdutoId = req.ProdutoId,
            ValorSolicitado = req.ValorSolicitado,
            PrazoMeses = req.PrazoMeses,
            Status = Domain.Enums.StatusContratacao.Pendente
        };
        _db.Contratacoes.Add(contratacao);
        await _db.SaveChangesAsync();

        await _queue.EnfileirarAsync(new ContratacaoMensagem(contratacao.Id));

        var resp = Mapear(contratacao);
        return AcceptedAtAction(nameof(ConsultarStatus), new { id = contratacao.Id }, resp);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ContratacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarStatus(int id)
    {
        var contratacao = await _db.Contratacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contratacao is null)
            return NotFound(new { mensagem = "Contratação não encontrada." });

        return Ok(Mapear(contratacao));
    }

    private static ContratacaoResponse Mapear(Contratacao c) => new(
        c.Id, c.ClienteId, c.ProdutoId, c.ValorSolicitado, c.PrazoMeses,
        c.Status.ToString(), c.MotivoStatus, c.ScoreCalculado,
        c.TaxaJurosAplicada, c.ValorAprovado,
        c.DataSolicitacao, c.DataProcessamento);
}
