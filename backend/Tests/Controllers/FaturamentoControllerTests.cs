using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using RedMobilePedidos.API.Controllers;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;
using RedMobilePedidos.Tests.Builders;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class FaturamentoControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<DashboardFaturamentoController>> _mockLogger = new();

    private DashboardFaturamentoController CreateController(MockHttpClientFactory factory)
        => new(factory, ProtheusSettingsTeste, _mockLogger.Object);
    [Fact]
    public async Task ObterFaturamentos_DeveRetornarFaturamentosPaginados()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var faturamentos = new List<FaturamentoItem>
        {
            TestDataBuilder.DashboardFaturamentoBuilder().Generate(),
            TestDataBuilder.DashboardFaturamentoBuilder().Generate()
        };
        var expectedDashboardFaturamentos = new RespostaPaginada<FaturamentoItem>
        {
            Dados = faturamentos,
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboardFaturamentos, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterFaturamentos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterFaturamentos_DeveRetornarListaVazia_QuandoNaoExistemFaturamentos()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var expectedDashboardFaturamentos = new RespostaPaginada<FaturamentoItem>
        {
            Dados = new List<FaturamentoItem>(),
            NumeroPagina = 1,
            TotalPaginas = 0
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboardFaturamentos, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterFaturamentos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterFaturamentos_DeveTratatarPaginacao()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 2,
            Tamanho = 5
        };
        var faturamentos = new List<FaturamentoItem>
        {
            TestDataBuilder.DashboardFaturamentoBuilder().Generate()
        };
        var expectedDashboardFaturamentos = new RespostaPaginada<FaturamentoItem>
        {
            Dados = faturamentos,
            NumeroPagina = 2,
            TotalPaginas = 3
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboardFaturamentos, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterFaturamentos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.NumeroPagina.Should().Be(2);
        result.TotalPaginas.Should().Be(3);
    }

    [Fact]
    public async Task ObterDashboardFaturamentos_DeveRetornarDadosDashboard()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var expectedDashboard = new DashboardFaturamento
        {
            TotalNF = 50000.00m,
            TotalBase = 45000.00m,
            QuantidadeVendas = 25,
            VendasPorCategoria = new List<ItemGrafico>
            {
                new()
                {
                    Rotulo = "Categoria A",
                    Series = new List<SerieGrafico>
                    {
                        new()
                        {
                            Nome = "Vendas",
                            Valor =15000.00
                        }
                    }
                }
            },
            ProdutosMaisVendidos = new List<ProdutoMaisVendido>
            {
                new()
                {
                    UrlImagem = "product1.jpg",
                    Nome = "Product 1",
                    Unidades = 100
                }
            },
            VendasPorDia = new List<ItemGrafico>
            {
                new()
                {
                    Rotulo = "01/06",
                    Series = new List<SerieGrafico>
                    {
                        new()
                        {
                            Nome = "Vendas",
                            Valor =2000.00
                        }
                    }
                }
            }
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboard, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterDashboardFaturamentos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalNF.Should().Be(50000.00m);
        result.TotalBase.Should().Be(45000.00m);
        result.QuantidadeVendas.Should().Be(25);
        result.VendasPorCategoria.Should().HaveCount(1);
        result.ProdutosMaisVendidos.Should().HaveCount(1);
        result.VendasPorDia.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObterDashboardFaturamentos_DeveRetornarListasVazias_QuandoNaoExistemDados()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var expectedDashboard = new DashboardFaturamento
        {
            TotalNF = 0m,
            TotalBase = 0m,
            QuantidadeVendas = 0,
            VendasPorCategoria = new List<ItemGrafico>(),
            ProdutosMaisVendidos = new List<ProdutoMaisVendido>(),
            VendasPorDia = new List<ItemGrafico>()
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboard, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterDashboardFaturamentos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalNF.Should().Be(0m);
        result.VendasPorCategoria.Should().BeEmpty();
        result.ProdutosMaisVendidos.Should().BeEmpty();
    }
}
