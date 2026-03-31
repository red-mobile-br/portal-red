using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using RedMobilePedidos.API.Controllers;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class PlanoPagamentoControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<PlanoPagamentoController>> _mockLogger = new();

    private PlanoPagamentoController CreateController(MockHttpClientFactory factory)
        => new(factory, _mockLogger.Object);
    [Fact]
    public async Task ObterTodos_DeveRetornarTodosPlanosPagamento()
    {
        // Arrange
        var expectedData = new RespostaPaginada<PlanoPagamento>
        {
            Dados = new List<PlanoPagamento>
            {
                new()
                {
                    Codigo = "001",
                    Descricao = "À Vista"
                },
                new()
                {
                    Codigo = "002",
                    Descricao = "30 Dias"
                },
                new()
                {
                    Codigo = "003",
                    Descricao = "60 Dias"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedData, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTodos(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Codigo == "001");
        result.Should().Contain(p => p.Descricao == "À Vista");
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarListaVazia_QuandoNaoExistemPlanosPagamento()
    {
        // Arrange
        var expectedData = new RespostaPaginada<PlanoPagamento>
        {
            Dados = new List<PlanoPagamento>(),
            NumeroPagina = 1,
            TotalPaginas = 0
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedData, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTodos(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarApenasPropriedadeDados()
    {
        // Arrange
        var planosPagamento = new List<PlanoPagamento>
        {
            new()
            {
                Codigo = "001",
                Descricao = "30/60/90 Dias"
            },
            new()
            {
                Codigo = "002",
                Descricao = "Boleto Bancário"
            }
        };

        var expectedData = new RespostaPaginada<PlanoPagamento>
        {
            Dados = planosPagamento,
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedData, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTodos(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(planosPagamento);
    }
}
