using System;
using System.Collections.Generic;
using System.Linq;
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
using RedMobilePedidos.API.Models.Responses.Titulos;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class TituloControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<TituloController>> _mockLogger = new();

    private TituloController CreateController(MockHttpClientFactory factory)
        => new(factory, ProtheusSettingsTeste, _mockLogger.Object);
    [Fact]
    public async Task ObterTitulos_DeveRetornarTitulosPaginados()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var titulosEsperados = new RespostaPaginada<TituloListaItem>
        {
            Dados = new List<TituloListaItem>
            {
                new()
                {
                    NumeroTitulo = "TITLE001",
                    NomeCliente = "Client ABC",
                    Parcela = 1,
                    Cnpj = "12345678000190",
                    NumeroPedido = "ORD001",
                    ValorTitulo = 1000.00m,
                    Status = 0,
                    DataVencimento = DateTime.Today.AddDays(30),
                    DataPagamento = null,
                    VencimentoOriginal = DateTime.Today.AddDays(30),
                    NomeRepresentante = "Vendor 1",
                    IdRepresentante = "V001"
                },
                new()
                {
                    NumeroTitulo = "TITLE002",
                    NomeCliente = "Client XYZ",
                    Parcela = 2,
                    Cnpj = "98765432000111",
                    NumeroPedido = "ORD002",
                    ValorTitulo = 2000.00m,
                    Status = 2,
                    DataVencimento = DateTime.Today.AddDays(-10),
                    DataPagamento = DateTime.Today.AddDays(-5),
                    VencimentoOriginal = DateTime.Today.AddDays(-10),
                    NomeRepresentante = "Vendor 2",
                    IdRepresentante = "V002"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(titulosEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTitulos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().HaveCount(2);
        result.Dados.First().NumeroTitulo.Should().Be("TITLE001");
    }

    [Fact]
    public async Task ObterTitulos_DeveRetornarListaVazia_QuandoNaoExistemTitulos()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var titulosEsperados = new RespostaPaginada<TituloListaItem>
        {
            Dados = new List<TituloListaItem>(),
            NumeroPagina = 1,
            TotalPaginas = 0
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(titulosEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTitulos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterTitulos_DeveTratarPaginacao()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 3,
            Tamanho = 20
        };
        var titulosEsperados = new RespostaPaginada<TituloListaItem>
        {
            Dados = new List<TituloListaItem>
            {
                new()
                {
                    NumeroTitulo = "TITLE050",
                    NomeCliente = "Client Test",
                    Parcela = 1,
                    Cnpj = "11111111000111",
                    NumeroPedido = "ORD050",
                    ValorTitulo = 500.00m,
                    Status = 0,
                    DataVencimento = DateTime.Today.AddDays(15),
                    DataPagamento = null,
                    VencimentoOriginal = DateTime.Today.AddDays(15),
                    NomeRepresentante = "Vendor Test",
                    IdRepresentante = "V999"
                }
            },
            NumeroPagina = 3,
            TotalPaginas = 5
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(titulosEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTitulos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.NumeroPagina.Should().Be(3);
        result.TotalPaginas.Should().Be(5);
    }

    [Fact]
    public async Task ObterDashboardTitulos_DeveRetornarDadosDashboard()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var expectedDashboard = new DashboardTitulos
        {
            ValorRecebido = 25000.00m,
            ValorAReceber = 15000.00m,
            DesempenhoPorPeriodo = new List<ItemGrafico>
            {
                new()
                {
                    Rotulo = "Janeiro",
                    Series = new List<SerieGrafico>
                    {
                        new()
                        {
                            Nome = "Recebido",
                            Valor =10000.00
                        },
                        new()
                        {
                            Nome = "A Receber",
                            Valor =5000.00
                        }
                    }
                },
                new()
                {
                    Rotulo = "Fevereiro",
                    Series = new List<SerieGrafico>
                    {
                        new()
                        {
                            Nome = "Recebido",
                            Valor =15000.00
                        },
                        new()
                        {
                            Nome = "A Receber",
                            Valor =10000.00
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
        var result = await controller.ObterDashboardTitulos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ValorRecebido.Should().Be(25000.00m);
        result.ValorAReceber.Should().Be(15000.00m);
        result.DesempenhoPorPeriodo.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterDashboardTitulos_DeveRetornarValoresZero_QuandoNaoExistemDados()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var expectedDashboard = new DashboardTitulos
        {
            ValorRecebido = 0m,
            ValorAReceber = 0m,
            DesempenhoPorPeriodo = new List<ItemGrafico>()
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboard, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterDashboardTitulos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ValorRecebido.Should().Be(0m);
        result.ValorAReceber.Should().Be(0m);
        result.DesempenhoPorPeriodo.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterTitulos_DeveIncluirDatasNulas()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var titulosEsperados = new RespostaPaginada<TituloListaItem>
        {
            Dados = new List<TituloListaItem>
            {
                new()
                {
                    NumeroTitulo = "TITLE003",
                    NomeCliente = "Client Test",
                    Parcela = 1,
                    Cnpj = "12345678000190",
                    NumeroPedido = "ORD003",
                    ValorTitulo = 3000.00m,
                    Status = 0,
                    DataVencimento = null,
                    DataPagamento = null,
                    VencimentoOriginal = null,
                    NomeRepresentante = "Vendor 3",
                    IdRepresentante = "V003"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(titulosEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTitulos(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().HaveCount(1);
        result.Dados.First().DataVencimento.Should().BeNull();
        result.Dados.First().DataPagamento.Should().BeNull();
        result.Dados.First().VencimentoOriginal.Should().BeNull();
    }
}
