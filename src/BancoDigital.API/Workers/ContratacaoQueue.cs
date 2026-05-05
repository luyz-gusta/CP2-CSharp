using System.Threading.Channels;
using BancoDigital.API.DTOs;

namespace BancoDigital.API.Workers;

public interface IContratacaoQueue
{
    ValueTask EnfileirarAsync(ContratacaoMensagem msg, CancellationToken ct = default);
    ValueTask<ContratacaoMensagem> DesenfileirarAsync(CancellationToken ct);
}

public class ContratacaoQueue : IContratacaoQueue
{
    private readonly Channel<ContratacaoMensagem> _channel = Channel.CreateUnbounded<ContratacaoMensagem>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

    public ValueTask EnfileirarAsync(ContratacaoMensagem msg, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(msg, ct);

    public ValueTask<ContratacaoMensagem> DesenfileirarAsync(CancellationToken ct)
        => _channel.Reader.ReadAsync(ct);
}
