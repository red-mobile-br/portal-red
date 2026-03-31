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
using RedMobilePedidos.API.Services;
using RedMobilePedidos.Tests.Builders;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class PedidoControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<PedidoController>> _mockLogger = new();
    private readonly Mock<IEmailService> _mockEmailService = new();
    private readonly Mock<IPdfService> _mockPdfService = new();
    private readonly Mock<IRelatorioPedidoService> _mockOrderReportService = new();
    private readonly Mock<IEmailTemplateService> _mockEmailTemplateService = new();

    private PedidoController CreateController(MockHttpClientFactory factory)
    {
        return new PedidoController(
            factory,
            _mockLogger.Object,
            _mockEmailService.Object,
            _mockPdfService.Object,
            _mockOrderReportService.Object,
            _mockEmailTemplateService.Object
        );
    }
    [Fact]
    public async Task PedidoDetalhados_DeveRetornarPedidosPaginados()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var pedidosEsperados = new RespostaPaginada<PedidoListaItem>
        {
            Dados = new List<PedidoListaItem>
            {
                new()
                {
                    Id = "ORD001",
                    Nome = "Client ABC",
                    ValorTotal = 1500.00m,
                    Status = "Open",
                    Cnpj = "123456",
                    IdCliente = "CLI001",
                    Notas = [],
                    NotasFiscais = [],
                    Boletos = [],
                    Representante = "V001",
                    NomeRepresentante = "Vendor 1",
                    DataLancamento = DateTime.Today
                },
                new()
                {
                    Id = "ORD002",
                    Nome = "Client XYZ",
                    ValorTotal = 2500.00m,
                    Status = "Open",
                    Cnpj = "654321",
                    IdCliente = "CLI002",
                    Notas = [],
                    NotasFiscais = [],
                    Boletos = [],
                    Representante = "V002",
                    NomeRepresentante = "Vendor 2",
                    DataLancamento = DateTime.Today
                }
            },
            NumeroPagina = 1,
            TotalPaginas = 1
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(pedidosEsperados, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.PedidoDetalhados(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task PedidoDetalhadoPorNumero_DeveRetornarOk_QuandoPedidoExiste()
    {
        // Arrange
        var numeroPedido = "ORD001";
        var pedidoEsperado = TestDataBuilder.OrderBuilder().Generate();

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(pedidoEsperado, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.PedidoDetalhadoPorNumero(numeroPedido, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(pedidoEsperado);
    }

    [Fact]
    public async Task PedidoDetalhadoPorNumero_DeveRetornarNaoEncontrado_QuandoPedidoNaoExiste()
    {
        // Arrange
        var numeroPedido = "INVALID";

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            "null"
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.PedidoDetalhadoPorNumero(numeroPedido, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CriarPedido_DeveRetornarCriado_QuandoBemSucedido()
    {
        // Arrange
        var novoPedido = new CriarPedido
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
            Id = "ORD123",
            Sucesso = true,
            Detalhes = "Order created successfully"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(respostaCriacao, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.CriarPedido(novoPedido, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(PedidoController.PedidoDetalhadoPorNumero));
        createdResult.Value.Should().BeEquivalentTo(respostaCriacao);
    }

    [Fact]
    public async Task PedidoDetalhadosDashboard_DeveRetornarDashboard()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };
        var expectedDashboard = new DashboardPedidos
        {
            TotalPedidosPeriodo = 50,
            PedidosAbertosPeriodo = 10,
            ValorTotalPedidos = 25000.00m,
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
        var result = await controller.PedidoDetalhadosDashboard(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalPedidosPeriodo.Should().Be(50);
        result.PedidosAbertosPeriodo.Should().Be(10);
    }

    [Fact]
    public async Task EnviarEmail_DeveRetornarOk_QuandoEmailEnviadoComSucesso()
    {
        // Arrange
        var numeroPedido = "ORD001";
        var pedidoEsperado = TestDataBuilder.OrderBuilder().Generate();
        var emailModel = new API.Models.Requests.EmailModel { Destinatario = "test@example.com" };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(pedidoEsperado, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);

        _mockOrderReportService.Setup(x => x.GerarRelatorioHtml(It.IsAny<PedidoDetalhado>()))
            .Returns("<html><body>Test Report</body></html>");
        _mockPdfService.Setup(x => x.GerarPdfDeHtmlAsync(It.IsAny<string>()))
            .ReturnsAsync(new byte[] { 0x25, 0x50, 0x44, 0x46 }); // PDF magic bytes
        _mockEmailService.Setup(x => x.EnviarEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<byte[]>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.EnviarEmail(numeroPedido, emailModel, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockEmailService.Verify(x => x.EnviarEmailAsync(
            "test@example.com",
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task EnviarEmail_DeveRetornarNaoEncontrado_QuandoPedidoNaoExiste()
    {
        // Arrange
        var numeroPedido = "INVALID";
        var emailModel = new API.Models.Requests.EmailModel { Destinatario = "test@example.com" };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            "null"
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.EnviarEmail(numeroPedido, emailModel, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ExcluirPedido_DeveRetornarSemConteudo_QuandoBemSucedido()
    {
        // Arrange
        var numeroPedido = "ORD001";
        var respostaExclusao = new RespostaPadrao
        {
            Sucesso = true,
            Detalhes = "Order deleted successfully"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(respostaExclusao, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ExcluirPedido(numeroPedido, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AtualizarPedido_DeveRetornarRespostaPadrao_QuandoBemSucedido()
    {
        // Arrange
        var numeroPedido = "ORD001";
        var pedidoAtualizado = new CriarPedido
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
            Detalhes = "Order updated successfully"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedResponse, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.AtualizarPedido(numeroPedido, pedidoAtualizado, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task AprovarPedido_DeveRetornarRespostaPadrao_QuandoBemSucedido()
    {
        // Arrange
        var numeroPedido = "ORD001";
        var expectedResponse = new RespostaPadrao
        {
            Sucesso = true,
            Detalhes = "Order approved"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedResponse, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.AprovarPedido(numeroPedido, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task RecusarPedido_DeveRetornarRespostaPadrao_QuandoBemSucedido()
    {
        // Arrange
        var numeroPedido = "ORD001";
        var expectedResponse = new RespostaPadrao
        {
            Sucesso = true,
            Detalhes = "Order declined"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(expectedResponse, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.RecusarPedido(numeroPedido, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
    }

    [Fact]
    public async Task PedidoDetalhadoPorNumero_DeveRetornarProdutosComValoresAbsolutosDeImpostos()
    {
        // Arrange
        var numeroPedido = "000123";
        var product = new ProdutoPedido
        {
            Ordem = 1,
            Id = "PROD01",
            Descricao = "Produto Teste",
            Quantidade = 2,
            PercentualDesconto = 5m,
            ValorUnitario = 100m,
            PrecoBase = 105m,
            Comissao = 3m,
            ValorTotal = 210m,
            Margem = 20m,
            PercentualICMS = 12m,
            PercentualIPI = 5m,
            PercentualICMSST = 3m,
            SaldoPendente = 0,
            ICMS = 25.20m,
            ICMSST = 6.30m,
            IPI = 10.50m,
            ComissaoMaxima = 5m
        };
        var pedidoEsperado = TestDataBuilder.OrderBuilder()
            .RuleFor(o => o.Produtos, _ => new List<ProdutoPedido> { product })
            .Generate();

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(pedidoEsperado, _jsonOpcoes)
        );
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.PedidoDetalhadoPorNumero(numeroPedido, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var order = okResult.Value.Should().BeOfType<PedidoDetalhado>().Subject;
        order.Produtos.Should().HaveCount(1);
        order.Produtos[0].ICMS.Should().Be(25.20m);
        order.Produtos[0].ICMSST.Should().Be(6.30m);
        order.Produtos[0].IPI.Should().Be(10.50m);
        order.Produtos[0].ComissaoMaxima.Should().Be(5m);
    }
}
