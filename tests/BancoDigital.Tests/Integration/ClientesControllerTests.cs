using System.Net;
using System.Net.Http.Json;
using BancoDigital.API.DTOs;
using BancoDigital.Tests.Infra;
using FluentAssertions;

namespace BancoDigital.Tests.Integration;

public class ClientesControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public ClientesControllerTests(TestWebAppFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task POST_pf_deve_cadastrar_pessoa_fisica()
    {
        var req = new CriarPessoaFisicaRequest(
            "Luiz Gustavo", "luiz@teste.com", "11999990000",
            "529.982.247-25", new DateTime(1995, 5, 10), 8000m, AgenciaId: 1);

        var resp = await _client.PostAsJsonAsync("/api/clientes/pf", req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var corpo = await resp.Content.ReadFromJsonAsync<ClienteResponse>();
        corpo!.Tipo.Should().Be("PF");
        corpo.Documento.Should().Be("52998224725");
    }

    [Fact]
    public async Task POST_pf_com_CPF_duplicado_deve_retornar_409()
    {
        var req = new CriarPessoaFisicaRequest(
            "Cliente A", "a@teste.com", "11", "111.444.777-35",
            new DateTime(1990, 1, 1), 5000m, AgenciaId: 1);

        var primeira = await _client.PostAsJsonAsync("/api/clientes/pf", req);
        primeira.StatusCode.Should().Be(HttpStatusCode.Created);

        var segunda = await _client.PostAsJsonAsync("/api/clientes/pf", req);
        segunda.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task POST_pf_com_agencia_inexistente_deve_retornar_404()
    {
        var req = new CriarPessoaFisicaRequest(
            "Sem Agencia", "sem@teste.com", "11", "248.438.034-80",
            new DateTime(1990, 1, 1), 5000m, AgenciaId: 9999);

        var resp = await _client.PostAsJsonAsync("/api/clientes/pf", req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_pf_com_CPF_invalido_deve_retornar_400()
    {
        var req = new CriarPessoaFisicaRequest(
            "Invalido", "x@teste.com", null, "111.111.111-11",
            new DateTime(1990, 1, 1), 5000m, AgenciaId: 1);

        var resp = await _client.PostAsJsonAsync("/api/clientes/pf", req);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_pj_deve_cadastrar_pessoa_juridica()
    {
        var req = new CriarPessoaJuridicaRequest(
            "Empresa X", "empresa@teste.com", "1133334444",
            "11.444.777/0001-61", "Empresa X LTDA",
            new DateTime(2015, 6, 1), 250000m, AgenciaId: 1);

        var resp = await _client.PostAsJsonAsync("/api/clientes/pj", req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var corpo = await resp.Content.ReadFromJsonAsync<ClienteResponse>();
        corpo!.Tipo.Should().Be("PJ");
        corpo.RazaoSocial.Should().Be("Empresa X LTDA");
    }

    [Fact]
    public async Task POST_pj_com_CNPJ_duplicado_deve_retornar_409()
    {
        var req = new CriarPessoaJuridicaRequest(
            "Empresa Y", "y@teste.com", "11", "45.997.418/0001-53",
            "Empresa Y SA", new DateTime(2010, 3, 15), 500000m, AgenciaId: 1);

        var primeira = await _client.PostAsJsonAsync("/api/clientes/pj", req);
        primeira.StatusCode.Should().Be(HttpStatusCode.Created);

        var segunda = await _client.PostAsJsonAsync("/api/clientes/pj", req);
        segunda.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GET_cliente_inexistente_deve_retornar_404()
    {
        var resp = await _client.GetAsync("/api/clientes/99999");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
