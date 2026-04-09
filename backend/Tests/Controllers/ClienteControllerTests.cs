using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using RedMobilePedidos.API.Controllers;
using RedMobilePedidos.API.Enums;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Clientes;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Models.Responses.Produtos;
using RedMobilePedidos.Tests.Builders;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class ClienteControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<ClienteController>> _mockLogger = new();

    private ClienteController CreateController(MockHttpClientFactory fabrica)
        => new(fabrica, ProtheusSettingsTeste, _mockLogger.Object);

    [Fact]
    public async Task ObterClientes_DeveRetornarClientesPaginados()
    {
        // Arrange
        var filtro = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var clientesEsperados = new RespostaPaginada<ClienteListaItem>
        {
            Dados = new List<ClienteListaItem>
            {
                new()
                {
                    Id = "CLI001",
                    Nome = "Cliente 1",
                    Cnpj = "12345678000190",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Status = StatusCliente.Ativo,
                    DiasSemComprar = 0,
                    IdRepresentante = "V001",
                    NomeRepresentante = "Representante 1"
                },
                new()
                {
                    Id = "CLI002",
                    Nome = "Cliente 2",
                    Cnpj = "98765432000111",
                    Cidade = "Rio de Janeiro",
                    UF = "RJ",
                    Status = StatusCliente.Ativo,
                    DiasSemComprar = 0,
                    IdRepresentante = "V002",
                    NomeRepresentante = "Representante 2"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(clientesEsperados, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.ObterClientes(filtro, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task Produtos_DeveRetornarProdutosPaginados()
    {
        // Arrange
        var idCliente = "CLI001";
        var filtro = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 20
        };
        var produtosEsperados = new RespostaPaginada<ProdutoListaItem>
        {
            Dados = new List<ProdutoListaItem>
            {
                new()
                {
                    Id = "PROD001",
                    Descricao = "Produto 1",
                    ValorUnitario = 100.00m
                },
                new()
                {
                    Id = "PROD002",
                    Descricao = "Produto 2",
                    ValorUnitario = 200.00m
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(produtosEsperados, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.Produtos(idCliente, filtro, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task Produto_DeveRetornarOk_QuandoProdutoExiste()
    {
        // Arrange
        var idCliente = "CLI001";
        var codigoProduto = "PROD001";
        var produtoEsperado = TestDataBuilder.ProductBuilder().Generate();

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(produtoEsperado, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.Produto(idCliente, codigoProduto, CancellationToken.None);

        // Assert
        resultado.Result.Should().BeOfType<OkObjectResult>();
        var resultadoOk = resultado.Result as OkObjectResult;
        resultadoOk!.Value.Should().BeEquivalentTo(produtoEsperado);
    }

    [Fact]
    public async Task Produto_DeveRetornarNaoEncontrado_QuandoProdutoNaoExiste()
    {
        // Arrange
        var idCliente = "CLI001";
        var codigoProduto = "INVALIDO";

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            "null"
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.Produto(idCliente, codigoProduto, CancellationToken.None);

        // Assert
        resultado.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ObterClientePorCnpj_DeveRetornarOk_QuandoClienteExiste()
    {
        // Arrange
        var cnpj = "12345678000190";
        var clienteEsperado = TestDataBuilder.ClientBuilder().Generate();

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(clienteEsperado, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.ObterClientePorCnpj(cnpj, CancellationToken.None);

        // Assert
        resultado.Result.Should().BeOfType<OkObjectResult>();
        var resultadoOk = resultado.Result as OkObjectResult;
        resultadoOk!.Value.Should().BeEquivalentTo(clienteEsperado);
    }

    [Fact]
    public async Task ObterClientePorCnpj_DeveRetornarNaoEncontrado_QuandoClienteNaoExiste()
    {
        // Arrange
        var cnpj = "99999999999999";

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            "null"
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.ObterClientePorCnpj(cnpj, CancellationToken.None);

        // Assert
        resultado.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ObterPedidosCliente_DeveRetornarPedidosPaginados()
    {
        // Arrange
        var cnpj = "12345678000190";
        var pedidosEsperados = new RespostaPaginada<PedidoListaItem>
        {
            Dados = new List<PedidoListaItem>
            {
                new()
                {
                    Id = "PED001",
                    DataEmissao = DateTime.Today,
                    NomeCliente = "Cliente ABC",
                    ValorTotal = 1000.00m,
                    Status = "Aberto",
                    Cnpj = cnpj,
                    IdCliente = "CLI001",
                    Anotacoes = [],
                    NotasFiscais = [],
                    Boletos = [],
                    IdRepresentante = "V001",
                    NomeRepresentante = "Representante 1"
                },
                new()
                {
                    Id = "PED002",
                    DataEmissao = DateTime.Today,
                    NomeCliente = "Cliente XYZ",
                    ValorTotal = 2000.00m,
                    Status = "Aberto",
                    Cnpj = cnpj,
                    IdCliente = "CLI001",
                    Anotacoes = [],
                    NotasFiscais = [],
                    Boletos = [],
                    IdRepresentante = "V002",
                    NomeRepresentante = "Representante 2"
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(pedidosEsperados, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.ObterPedidosCliente(cnpj, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterDashboardClientes_DeveRetornarDashboard()
    {
        // Arrange
        var filtro = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var dashboardEsperado = new DashboardClientes
        {
            ClientesPorEstado = [],
            ClientesPorEstatus = [],
            MelhoresClientesPorPedidos = [],
            MelhoresClientesPorFaturamento = []
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(dashboardEsperado, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.ObterDashboardClientes(filtro, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.ClientesPorEstado.Should().NotBeNull();
    }

    [Fact]
    public async Task ImpostoProduto_DeveRetornarImpostos_QuandoProdutoExiste()
    {
        // Arrange
        var idCliente = "CLI001";
        var codigoProduto = "PROD001";
        var modeloConsulta = new API.Models.Requests.ConsultaImpostoProduto
        {
            Quantidade = 10,
            PrecoBase = 100,
            PrecoVenda = 95
        };
        var impostosEsperados = new ImpostoProduto
        {
            ICMS = 18,
            ICMSST = 0,
            IPI = 10,
            ComissaoMaxima = 5,
            Comissao = 3,
            Margem = 5,
            PercentualICMS = 18,
            PercentualIPI = 10,
            PercentualICMSST = 0
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(impostosEsperados, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.ImpostoProduto(idCliente, codigoProduto, modeloConsulta, CancellationToken.None);

        // Assert
        resultado.Result.Should().BeOfType<OkObjectResult>();
        var resultadoOk = resultado.Result as OkObjectResult;
        var impostos = resultadoOk!.Value as ImpostoProduto;
        impostos!.ICMS.Should().Be(18);
        impostos.IPI.Should().Be(10);
    }
}
