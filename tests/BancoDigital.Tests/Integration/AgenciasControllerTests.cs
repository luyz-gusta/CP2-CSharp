using System.Net;
using System.Net.Http.Json;
using BancoDigital.API.DTOs;
using BancoDigital.Tests.Infra;
using FluentAssertions;

namespace BancoDigital.Tests.Integration;

public class AgenciasControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AgenciasControllerTests(TestWebAppFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task POST_agencia_deve_criar()
    {
        var req = new CriarAgenciaRequest("Agência Norte", "0099", "Av. Norte, 100");
        var resp = await _client.PostAsJsonAsync("/api/agencias", req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var corpo = await resp.Content.ReadFromJsonAsync<AgenciaResponse>();
        corpo!.Codigo.Should().Be("0099");
    }

    [Fact]
    public async Task POST_agencia_com_codigo_duplicado_deve_retornar_409()
    {
        var req = new CriarAgenciaRequest("Agência Z", "0123", "Endereço");
        await _client.PostAsJsonAsync("/api/agencias", req);
        var segunda = await _client.PostAsJsonAsync("/api/agencias", req);

        segunda.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GET_agencia_inexistente_deve_retornar_404()
    {
        var resp = await _client.GetAsync("/api/agencias/99999");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
