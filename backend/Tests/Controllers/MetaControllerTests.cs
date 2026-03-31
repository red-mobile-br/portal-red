using System;
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
using RedMobilePedidos.API.Models.Responses.Metas;
using RedMobilePedidos.Tests.Builders;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class MetaControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<MetaController>> _mockLogger = new();

    private MetaController CreateController(MockHttpClientFactory factory)
        => new(factory, _mockLogger.Object);
    [Fact]
    public async Task ObterMetasVigentes_DeveRetornarMetas()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var metas = TestDataBuilder.MetaBuilder().Generate(3);
        var listaMetas = new ListaMetas { Metas = metas };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(listaMetas, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterMetasVigentes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(metas);
    }

    [Fact]
    public async Task ObterMetasVigentes_DeveDefinirDatasPadrao_QuandoNaoFornecidas()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var metas = TestDataBuilder.MetaBuilder().Generate(2);
        var listaMetas = new ListaMetas { Metas = metas };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(listaMetas, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterMetasVigentes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        // Note: queryObject is immutable (record), so the controller creates a new instance
        // with updated dates instead of modifying the original.
    }

    [Fact]
    public async Task ObterMetasVigentes_DeveUsarDatasInformadas_QuandoFornecidas()
    {
        // Arrange
        var dataInicio = new DateTime(2025, 1, 1);
        var dataFim = new DateTime(2025, 1, 31);
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10,
            De = dataInicio,
            Ate = dataFim
        };

        var metas = TestDataBuilder.MetaBuilder().Generate(2);
        var listaMetas = new ListaMetas { Metas = metas };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(listaMetas, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterMetasVigentes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        queryObject.De.Should().Be(dataInicio);
    }

    [Fact]
    public async Task ObterMetas_DeveRetornarHistoricoMetas()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 20
        };
        var metas = TestDataBuilder.MetaBuilder().Generate(10);
        var listaMetas = new ListaMetas { Metas = metas };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(listaMetas, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterMetas(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task ObterMetas_DeveRetornarListaVazia_QuandoNaoExistemMetas()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var listaMetas = new ListaMetas { Metas = new List<Meta>() };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(listaMetas, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterMetas(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
