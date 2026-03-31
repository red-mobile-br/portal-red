using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RedMobilePedidos.Tests.Integration;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ApplicationFactory_ShouldStart_Successfully()
    {
        // Arrange & Act
        var clienteHttp = _factory.CreateClient();

        // Assert
        clienteHttp.Should().NotBeNull();
    }

    [Fact]
    public async Task Ping_ShouldReturnPong()
    {
        // Arrange
        var clienteHttp = _factory.CreateClient();

        // Act
        var resposta = await clienteHttp.GetAsync("/api/ping");
        var conteudo = await resposta.Content.ReadAsStringAsync();

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        conteudo.Should().Be("Pong!!");
    }

    [Fact(Skip = "Swagger temporariamente desabilitado")]
    public async Task Swagger_ShouldBeAccessible()
    {
        // Arrange
        var clienteHttp = _factory.CreateClient();

        // Act
        var resposta = await clienteHttp.GetAsync("/swagger/index.html");

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requer configuração de autenticação")]
    public async Task ApiEndpoint_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var clienteHttp = _factory.CreateClient();

        // Act
        var resposta = await clienteHttp.GetAsync("/api/clientes");

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
