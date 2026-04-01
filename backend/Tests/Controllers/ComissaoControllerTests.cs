using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using RedMobilePedidos.API.Controllers;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Comissao;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.Tests.Builders;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class ComissaoControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<ComissaoController>> _mockLogger = new();

    private ComissaoController CreateController(MockHttpClientFactory factory)
        => new(factory, ProtheusSettingsTeste, _mockLogger.Object);
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    public async Task ObterComissoes_DeveRetornarComissoes_QuandoStatusValido(string status)
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10,
            Status = status
        };
        var comissoes = TestDataBuilder.CommissionBuilder().Generate(5);
        var respostaPaginada = new RespostaPaginada<ComissaoListaItem>
        {
            Dados = comissoes,
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(respostaPaginada, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterComissoes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().HaveCount(5);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("3")]
    [InlineData("invalid")]
    public async Task ObterComissoes_DeveLancarExcecao_QuandoStatusInvalido(string? status)
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10,
            Status = status
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(HttpStatusCode.OK, "{}");
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.ObterComissoes(queryObject, CancellationToken.None)
        );

        exception.Message.Should().Contain("Status inválido");
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    public async Task ObterDashboardComissoes_DeveRetornarDashboard_QuandoStatusValido(string status)
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10,
            Status = status
        };
        var expectedDashboard = new DashboardComissoes
        {
            TotalComissaoPeriodo = 5000.00m,
            PercentualComissaoPeriodo = 10.5m,
            ComissoesPeriodo = [],
            MaioresComissoesPeriodo = []
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboard, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterDashboardComissoes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalComissaoPeriodo.Should().Be(5000.00m);
        result.PercentualComissaoPeriodo.Should().Be(10.5m);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("3")]
    public async Task ObterDashboardComissoes_DeveLancarExcecao_QuandoStatusInvalido(string? status)
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10,
            Status = status
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(HttpStatusCode.OK, "{}");
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.ObterDashboardComissoes(queryObject, CancellationToken.None)
        );

        exception.Message.Should().Contain("Status inválido");
    }
}
