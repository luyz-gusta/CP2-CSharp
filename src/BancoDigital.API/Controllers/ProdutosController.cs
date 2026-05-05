using BancoDigital.API.Data;
using BancoDigital.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProdutosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var produtos = await _db.Produtos.AsNoTracking().ToListAsync();
        return Ok(produtos.Select(Mapear));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var produto = await _db.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (produto is null) return NotFound(new { mensagem = "Produto não encontrado." });
        return Ok(Mapear(produto));
    }

    private static object Mapear(Produto p) => p switch
    {
        Emprestimo e => new
        {
            e.Id, e.Nome, e.Descricao, e.Ativo, Tipo = e.TipoProduto,
            e.ValorMinimo, e.ValorMaximo, e.PrazoMaximoMeses, e.TaxaJurosBaseAoMes
        },
        MaquinaDeCartao m => new
        {
            m.Id, m.Nome, m.Descricao, m.Ativo, Tipo = m.TipoProduto,
            m.Modelo, m.TaxaMdrDebito, m.TaxaMdrCredito, m.AluguelMensal
        },
        ReceberSalario r => new
        {
            r.Id, r.Nome, r.Descricao, r.Ativo, Tipo = r.TipoProduto,
            r.EmpresaConveniada, r.CnpjEmpregador
        },
        _ => new { p.Id, p.Nome, p.Descricao, p.Ativo, Tipo = p.TipoProduto }
    };
}
