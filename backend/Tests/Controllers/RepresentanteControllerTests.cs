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
using RedMobilePedidos.API.Models.Responses.Representantes;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class RepresentanteControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<RepresentantesController>> _mockLogger = new();

    private RepresentantesController CreateController(MockHttpClientFactory factory)
        => new(factory, _mockLogger.Object);
    [Fact]
    public async Task ObterRepresentantes_DeveRetornarRepresentantesPaginados()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var representantesEsperados = new RespostaPaginada<RepresentanteModel>
        {
            Dados = new List<RepresentanteModel>
            {
                new()
                {
                    Id = "V001",
                    Nome = "Vendor 1",
                    Cnpj = "12345678000190"
                },
                new()
                {
                    Id = "V002",
                    Nome = "Vendor 2",
                    Cnpj = "98765432000111"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(representantesEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterRepresentantes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterRepresentantes_DeveRetornarListaVazia_QuandoNaoExistemRepresentantes()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var representantesEsperados = new RespostaPaginada<RepresentanteModel>
        {
            Dados = new List<RepresentanteModel>(),
            NumeroPagina = 1,
            TotalPaginas = 0
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(representantesEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterRepresentantes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Dados.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterRepresentantes_DeveTratarPaginacao()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 2,
            Tamanho = 5
        };
        var representantesEsperados = new RespostaPaginada<RepresentanteModel>
        {
            Dados = new List<RepresentanteModel>
            {
                new()
                {
                    Id = "V006",
                    Nome = "Vendor 6",
                    Cnpj = "11111111000111"
                }
            },
            NumeroPagina = 2,
            TotalPaginas = 3
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(representantesEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterRepresentantes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.NumeroPagina.Should().Be(2);
        result.Value.TotalPaginas.Should().Be(3);
    }

    [Fact]
    public async Task ObterRepresentantes_DeveRetornarRepresentantesComTodasPropriedades()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var representantesEsperados = new RespostaPaginada<RepresentanteModel>
        {
            Dados = new List<RepresentanteModel>
            {
                new()
                {
                    Id = "V999",
                    Nome = "Complete Vendor Name",
                    Cnpj = "12345678000190"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(representantesEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterRepresentantes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        var representante = result.Value!.Dados.First();
        representante.Id.Should().Be("V999");
        representante.Nome.Should().Be("Complete Vendor Name");
        representante.Cnpj.Should().Be("12345678000190");
    }

    [Fact]
    public async Task ObterRepresentantes_DeveUsarUsuarioLogadoNaRequisicao()
    {
        // Arrange
        var customUserId = "CUSTOM_USER_123";
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var representantesEsperados = new RespostaPaginada<RepresentanteModel>
        {
            Dados = new List<RepresentanteModel>
            {
                new()
                {
                    Id = "V001",
                    Nome = "Vendor 1",
                    Cnpj = "12345678000190"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(representantesEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller, customUserId);

        // Act
        var result = await controller.ObterRepresentantes(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
    }
}
