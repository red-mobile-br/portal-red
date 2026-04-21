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
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Models.Responses.Orcamentos;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class OrcamentoControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<OrcamentoController>> _mockLogger = new();

    private OrcamentoController CreateController(MockHttpClientFactory factory)
        => new(factory, ProtheusSettingsTeste, _mockLogger.Object);
    [Fact]
    public async Task OrcamentoDetalhados_DeveRetornarOrcamentosPaginados()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var orcamentosEsperados = new RespostaPaginada<OrcamentoListaItem>
        {
            Dados = new List<OrcamentoListaItem>
            {
                new()
                {
                    Id = "QUO001",
                    NomeCliente = "Client ABC",
                    ValorTotal = 1500.00m,
                    IdPedido = "ORD001",
                    Status = "Open",
                    Cnpj = "123456",
                    IdCliente = "CLI001",
                    Anotacoes = [],
                    NotasFiscais = [],
                    Boletos = [],
                    IdRepresentante = "V001",
                    NomeRepresentante = "Vendor 1",
                    DataEmissao = DateTime.Today,
                    DataImplementacao = DateTime.Today,
                    DataCancelamento = DateTime.Today
                },
                new()
                {
                    Id = "QUO002",
                    NomeCliente = "Client XYZ",
                    ValorTotal = 2500.00m,
                    IdPedido = "ORD002",
                    Status = "Open",
                    Cnpj = "654321",
                    IdCliente = "CLI002",
                    Anotacoes = [],
                    NotasFiscais = [],
                    Boletos = [],
                    IdRepresentante = "V002",
                    NomeRepresentante = "Vendor 2",
                    DataEmissao = DateTime.Today,
                    DataImplementacao = DateTime.Today,
                    DataCancelamento = DateTime.Today
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(orcamentosEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.OrcamentoDetalhados(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task OrcamentoDetalhadoPorNumero_DeveRetornarOk_QuandoOrcamentoExiste()
    {
        // Arrange
        var numeroOrcamento = "QUO001";
        var orcamentoEsperado = new OrcamentoDetalhado
        {
            Id = numeroOrcamento,
            Cliente = new ClientePedido
            {
                Id = "CLI001",
                RazaoSocial = "Test Client",
                NomeFantasia = "",
                Cnpj = "",
                Endereco = new Endereco()
            },
            ValorNota = 5000.00m,
            ModoFrete = ModoFrete.CIF,
            DadosAgendamento = new DadosAgendamento
            {
                Email = "",
                Telefone = ""
            },
            EnderecoEntrega = new Endereco(),
            Produtos = [],
            NumeroPedidoCompra = "",
            DataEmissao = DateTime.Today,
            InformacoesNota = "",
            ObservacoesPedido = "",
            Anotacoes = [],
            ValorProdutos = 0,
            ValorICMS = 0,
            ValorICMSST = 0,
            ValorIPI = 0,
            NotasFiscais = [],
            Boletos = [],
            Status = "",
            PlanoPagamento = "",
            TipoPedido = 0,
            IdRepresentante = "",
            NomeRepresentante = "",
            MargemPedido = 0,
            IdGerente = "",
            NomeGerente = "",
            PesoLiquido = 0,
            PesoBruto = 0,
            DataImplementacao = DateTime.Today,
            DataCancelamento = DateTime.Today
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(orcamentoEsperado, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.OrcamentoDetalhadoPorNumero(numeroOrcamento, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(orcamentoEsperado);
    }

    [Fact]
    public async Task OrcamentoDetalhadoPorNumero_DeveRetornarNaoEncontrado_QuandoOrcamentoNaoExiste()
    {
        // Arrange
        var numeroOrcamento = "INVALID";

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            "null"
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.OrcamentoDetalhadoPorNumero(numeroOrcamento, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CriarOrcamento_DeveRetornarCriado_QuandoBemSucedido()
    {
        // Arrange
        var novoOrcamento = new CriarPedido
        {
            IdCliente = "CLI001",
            EnderecoEntrega = new Endereco(),
            Produtos = [],
            ModoFrete = ModoFrete.CIF,
            DataEmissao = DateTime.Today,
            TipoPedido = TipoPedido.Venda
        };

        var respostaCriacao = new RespostaCriarPedido
        {
            Id = "QUO123",
            Sucesso = true,
            Detalhes = "Quote created successfully"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(respostaCriacao, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.CriarOrcamento(novoOrcamento, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(OrcamentoController.OrcamentoDetalhadoPorNumero));
        createdResult.Value.Should().BeEquivalentTo(respostaCriacao);
    }

    [Fact]
    public async Task AtualizarOrcamento_DeveRetornarRespostaPadrao_QuandoBemSucedido()
    {
        // Arrange
        var numeroOrcamento = "QUO001";
        var orcamentoAtualizado = new CriarPedido
        {
            IdCliente = "CLI001",
            EnderecoEntrega = new Endereco(),
            Produtos = [],
            ModoFrete = ModoFrete.CIF,
            DataEmissao = DateTime.Today,
            TipoPedido = TipoPedido.Venda
        };
        var expectedResponse = new RespostaPadrao
        {
            Sucesso = true,
            Detalhes = "Quote updated successfully"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedResponse, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.AtualizarOrcamento(numeroOrcamento, orcamentoAtualizado, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task ExcluirOrcamento_DeveRetornarSemConteudo_QuandoBemSucedido()
    {
        // Arrange
        var numeroOrcamento = "QUO001";
        var respostaExclusao = new RespostaPadrao
        {
            Sucesso = true,
            Detalhes = "Quote deleted successfully"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(respostaExclusao, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ExcluirOrcamento(numeroOrcamento, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AprovarOrcamento_DeveRetornarRespostaPadrao_QuandoBemSucedido()
    {
        // Arrange
        var numeroOrcamento = "QUO001";
        var expectedResponse = new RespostaPadrao
        {
            Sucesso = true,
            Detalhes = "Quote approved and converted to order"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedResponse, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.AprovarOrcamento(numeroOrcamento, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task RecusarOrcamento_DeveRetornarRespostaPadrao_QuandoBemSucedido()
    {
        // Arrange
        var numeroOrcamento = "QUO001";
        var expectedResponse = new RespostaPadrao
        {
            Sucesso = true,
            Detalhes = "Quote declined"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedResponse, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.RecusarOrcamento(numeroOrcamento, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task OrcamentoDetalhadosDashboard_DeveRetornarDashboardPedidos()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery { Pagina = 1, Tamanho = 10 };
        var expectedDashboard = new DashboardPedidos
        {
            TotalPedidosNoPeriodo = 10,
            PedidosAbertosNoPeriodo = 5,
            ValorTotalPedidos = 50000m,
            PedidosPorPeriodo = [],
            PedidosPorTipo = []
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedDashboard, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.OrcamentoDetalhadosDashboard(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalPedidosNoPeriodo.Should().Be(10);
    }
}
