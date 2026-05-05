using BancoDigital.API.Data;
using BancoDigital.API.Domain.Entities;
using BancoDigital.API.Domain.Enums;
using BancoDigital.API.Services;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Workers;

public class ContratacaoWorker : BackgroundService
{
    private readonly IContratacaoQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ContratacaoWorker> _logger;

    public ContratacaoWorker(
        IContratacaoQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<ContratacaoWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ContratacaoWorker iniciado.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var msg = await _queue.DesenfileirarAsync(stoppingToken);
                await ProcessarAsync(msg.ContratacaoId, stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no worker de contratação.");
            }
        }
    }

    private async Task ProcessarAsync(int contratacaoId, CancellationToken ct)
    {
        await Task.Delay(500, ct); // simula latência de análise

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var analise = scope.ServiceProvider.GetRequiredService<AnaliseCreditoService>();

        var contratacao = await db.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == contratacaoId, ct);

        if (contratacao is null)
        {
            _logger.LogWarning("Contratação {Id} não encontrada.", contratacaoId);
            return;
        }

        contratacao.Status = StatusContratacao.EmAnalise;
        await db.SaveChangesAsync(ct);

        try
        {
            if (contratacao.Produto is Emprestimo emp && contratacao.Cliente is not null)
            {
                var resultado = analise.Analisar(contratacao.Cliente, emp,
                    contratacao.ValorSolicitado, contratacao.PrazoMeses);

                contratacao.ScoreCalculado = resultado.Score;
                contratacao.MotivoStatus = resultado.Motivo;
                contratacao.Status = resultado.Aprovado
                    ? StatusContratacao.Aprovada
                    : StatusContratacao.Reprovada;

                if (resultado.Aprovado)
                {
                    contratacao.TaxaJurosAplicada = resultado.TaxaJurosAplicada;
                    contratacao.ValorAprovado = resultado.ValorAprovado;
                }
            }
            else
            {
                contratacao.Status = StatusContratacao.Aprovada;
                contratacao.MotivoStatus = "Produto sem regra de análise específica.";
                contratacao.ValorAprovado = contratacao.ValorSolicitado;
            }

            contratacao.DataProcessamento = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);

            _logger.LogInformation("Contratação {Id} processada com status {Status}.",
                contratacao.Id, contratacao.Status);
        }
        catch (Exception ex)
        {
            contratacao.Status = StatusContratacao.Falha;
            contratacao.MotivoStatus = $"Falha no processamento: {ex.Message}";
            contratacao.DataProcessamento = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            _logger.LogError(ex, "Falha ao processar contratação {Id}.", contratacaoId);
        }
    }
}
