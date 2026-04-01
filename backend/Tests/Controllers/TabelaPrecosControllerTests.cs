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
using RedMobilePedidos.API.Models.Responses.TabelaPrecoss;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class TabelaPrecosControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<TabelaPrecosController>> _mockLogger = new();

    private TabelaPrecosController CreateController(MockHttpClientFactory factory)
        => new(factory, ProtheusSettingsTeste, _mockLogger.Object);
    [Fact]
    public async Task ObterTabelaPrecos_DeveRetornarTabelaPrecosParaEstado()
    {
        // Arrange
        var estado = "SP";
        var tabelaPrecosEsperada = new TabelaPrecos
        {
            Descricao = "Tabela de Preços São Paulo",
            DataAtualizacao = DateTime.Today,
            Comentarios = "Preços válidos para 2024",
            ValorMinimoFreteCif = 1000.00m,
            Prazo = "30 dias",
            ICMS = 18.00m,
            Produtos = new List<ProdutoTabelaPrecos>
            {
                new()
                {
                    ReferenciaComercial = "PROD001",
                    Descricao = "Product 1",
                    Imagem = "image1.jpg",
                    ComissaoPorcento3 = new ComissaoTabelaPrecos
                    {
                        PrecoSemIpi = 100.00m,
                        PercentualIPI = 10.00m,
                        PrecoComIpi = 110.00m,
                        UnidadesPorEmbalagem = 12
                    },
                    ComissaoPorcento5 = new ComissaoTabelaPrecos
                    {
                        PrecoSemIpi = 95.00m,
                        PercentualIPI = 10.00m,
                        PrecoComIpi = 104.50m,
                        UnidadesPorEmbalagem = 12
                    },
                    PrecoMercado = 150.00m,
                    PercentualIPI = 10.00m
                }
            }
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tabelaPrecosEsperada, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTabelaPrecos(estado, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Descricao.Should().Be("Tabela de Preços São Paulo");
        result.ICMS.Should().Be(18.00m);
        result.Produtos.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObterTabelaPrecos_DeveTratarEstadoEmMinusculas()
    {
        // Arrange
        var estado = "rj";
        var tabelaPrecosEsperada = new TabelaPrecos
        {
            Descricao = "Tabela de Preços Rio de Janeiro",
            DataAtualizacao = DateTime.Today,
            Comentarios = "Preços atualizados",
            ValorMinimoFreteCif = 800.00m,
            Prazo = "45 dias",
            ICMS = 0.00m,
            Produtos = new List<ProdutoTabelaPrecos>()
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tabelaPrecosEsperada, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTabelaPrecos(estado, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Descricao.Should().Contain("Rio de Janeiro");
    }

    [Fact]
    public async Task ObterTabelaPrecos_DeveRetornarTabelaPrecosComProdutosVazios()
    {
        // Arrange
        var estado = "MG";
        var tabelaPrecosEsperada = new TabelaPrecos
        {
            Descricao = "Tabela de Preços Minas Gerais",
            DataAtualizacao = DateTime.Today.AddDays(-5),
            Comentarios = "Sem produtos disponíveis",
            ValorMinimoFreteCif = 500.00m,
            Prazo = "60 dias",
            ICMS = 8.00m,
            Produtos = new List<ProdutoTabelaPrecos>()
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tabelaPrecosEsperada, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTabelaPrecos(estado, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Produtos.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterTabelaPrecos_DeveIncluirTodasPropriedadesTabelaPrecos()
    {
        // Arrange
        var estado = "RS";
        var tabelaPrecosEsperada = new TabelaPrecos
        {
            Descricao = "Tabela de Preços Rio Grande do Sul",
            DataAtualizacao = new DateTime(2024, 6, 15),
            Comentarios = "Tabela atualizada mensalmente",
            ValorMinimoFreteCif = 1500.00m,
            Prazo = "30/60/90 dias",
            ICMS = 7.00m,
            Produtos = new List<ProdutoTabelaPrecos>
            {
                new()
                {
                    ReferenciaComercial = "REF123",
                    Descricao = "Produto Teste",
                    Imagem = "test.png",
                    ComissaoPorcento3 = new ComissaoTabelaPrecos
                    {
                        PrecoSemIpi = 200.00m,
                        PercentualIPI = 5.00m,
                        PrecoComIpi = 210.00m,
                        UnidadesPorEmbalagem = 6
                    },
                    ComissaoPorcento5 = new ComissaoTabelaPrecos
                    {
                        PrecoSemIpi = 190.00m,
                        PercentualIPI = 5.00m,
                        PrecoComIpi = 199.50m,
                        UnidadesPorEmbalagem = 6
                    },
                    PrecoMercado = 250.00m,
                    PercentualIPI = 5.00m
                }
            }
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(tabelaPrecosEsperada, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterTabelaPrecos(estado, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ValorMinimoFreteCif.Should().Be(1500.00m);
        result.Prazo.Should().Be("30/60/90 dias");
        result.DataAtualizacao.Should().Be(new DateTime(2024, 6, 15));
        result.Produtos.First().ComissaoPorcento3.UnidadesPorEmbalagem.Should().Be(6);
        result.Produtos.First().ComissaoPorcento5.PrecoComIpi.Should().Be(199.50m);
    }
}
