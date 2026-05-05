using System.Net;
using System.Net.Http.Json;
using BancoDigital.API.DTOs;
using BancoDigital.Tests.Infra;
using FluentAssertions;

namespace BancoDigital.Tests.Integration;

public class ContratacoesControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public ContratacoesControllerTests(TestWebAppFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task POST_contratacao_valida_deve_retornar_202_e_publicar_na_fila()
    {
        var pf = await CriarClientePfAsync("390.533.447-05");

        var req = new CriarContratacaoRequest(
            ClienteId: pf.Id, ProdutoId: 1, ValorSolicitado: 5000m, PrazoMeses: 24);

        var resp = await _client.PostAsJsonAsync("/api/contratacoes", req);

        resp.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var corpo = await resp.Content.ReadFromJsonAsync<ContratacaoResponse>();
        corpo!.Status.Should().BeOneOf("Pendente", "EmAnalise", "Aprovada", "Reprovada");
    }

    [Fact]
    public async Task POST_contratacao_para_cliente_inexistente_deve_retornar_404()
    {
        var req = new CriarContratacaoRequest(
            ClienteId: 99999, ProdutoId: 1, ValorSolicitado: 1000m, PrazoMeses: 12);

        var resp = await _client.PostAsJsonAsync("/api/contratacoes", req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_contratacao_para_produto_inexistente_deve_retornar_404()
    {
        var pf = await CriarClientePfAsync("111.222.333-96");

        var req = new CriarContratacaoRequest(
            ClienteId: pf.Id, ProdutoId: 99999, ValorSolicitado: 1000m, PrazoMeses: 12);

        var resp = await _client.PostAsJsonAsync("/api/contratacoes", req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_status_apos_processamento_deve_retornar_status_final()
    {
        var pf = await CriarClientePfAsync("444.555.666-19", renda: 12000m);

        var req = new CriarContratacaoRequest(
            ClienteId: pf.Id, ProdutoId: 1, ValorSolicitado: 8000m, PrazoMeses: 24);

        var post = await _client.PostAsJsonAsync("/api/contratacoes", req);
        post.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var criada = await post.Content.ReadFromJsonAsync<ContratacaoResponse>();

        ContratacaoResponse? final = null;
        for (int i = 0; i < 30; i++)
        {
            await Task.Delay(300);
            var get = await _client.GetAsync($"/api/contratacoes/{criada!.Id}");
            get.StatusCode.Should().Be(HttpStatusCode.OK);
            final = await get.Content.ReadFromJsonAsync<ContratacaoResponse>();
            if (final!.Status is "Aprovada" or "Reprovada" or "Falha") break;
        }

        final!.Status.Should().BeOneOf("Aprovada", "Reprovada");
        final.DataProcessamento.Should().NotBeNull();
        final.ScoreCalculado.Should().NotBeNull();
    }

    [Fact]
    public async Task GET_contratacao_inexistente_deve_retornar_404()
    {
        var resp = await _client.GetAsync("/api/contratacoes/99999");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<ClienteResponse> CriarClientePfAsync(string cpf, decimal renda = 5000m)
    {
        var req = new CriarPessoaFisicaRequest(
            $"Cliente {Guid.NewGuid():N}".Substring(0, 20),
            $"{Guid.NewGuid():N}@teste.com", null, cpf,
            new DateTime(1990, 1, 1), renda, AgenciaId: 1);

        var resp = await _client.PostAsJsonAsync("/api/clientes/pf", req);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ClienteResponse>())!;
    }
}
