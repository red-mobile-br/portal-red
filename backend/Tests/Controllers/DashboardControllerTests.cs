using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using RedMobilePedidos.API.Controllers;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Clientes;
using RedMobilePedidos.API.Models.Responses.Comissao;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class DashboardControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly Mock<ILogger<DashboardController>> _mockLogger = new();

    private DashboardController CreateController(MockHttpClientFactory factory)
        => new(factory, ProtheusSettingsTeste, _mockLogger.Object);
    [Fact]
    public async Task GetDashboard_ShouldReturnCombinedDashboardData()
    {
        // Arrange
        var queryObject = new FiltroPadraoQuery
        {
            Pagina = 1,
            Tamanho = 10
        };

        var orderDashboard = new DashboardPedidos
        {
            TotalPedidosNoPeriodo = 100,
            PedidosAbertosNoPeriodo = 25,
            ValorTotalPedidos = 50000.00m,
            PedidosPorPeriodo = [],
            PedidosPorTipo = []
        };

        var commissionDashboard = new DashboardComissoes
        {
            TotalComissaoPorPeriodo = 5000.00m,
            PercentualComissaoPorPeriodo = 10.5m,
            ComissoesPorPeriodo = [],
            MaiorComissaoPorPeriodo = []
        };

        var clientDashboard = new DashboardClientes
        {
            ClientesPorEstado =
            [
                new() { Uf ="SP", QuantidadeClientes = 10 },
                new() { Uf ="RJ", QuantidadeClientes = 5 }
            ],
            ClientesPorStatus = [],
            MelhoresClientesPorPedidos = [],
            MelhoresClientesPorFaturamento = []
        };

        var callCount = 0;
        var mockHandler = new MockHttpMessageHandler((request, cancellationToken) =>
        {
            callCount++;
            var response = callCount switch
            {
                1 => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(orderDashboard, _jsonOpcoes))
                },
                2 => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(commissionDashboard, _jsonOpcoes))
                },
                _ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(clientDashboard, _jsonOpcoes))
                }
            };
            return Task.FromResult(response);
        });

        var mockClient = new HttpClient(mockHandler);
        var factory = new MockHttpClientFactory(mockClient);
        var controller = CreateController(factory);
        SetupControllerContext(controller);

        // Act
        var result = await controller.ObterDashboard(queryObject, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalPedidosNoPeriodo.Should().Be(100);
        result.PedidosAbertosNoPeriodo.Should().Be(25);
        result.TotalComissaoPeriodo.Should().Be(5000.00m);
        result.ClientesPorEstado.Should().HaveCount(2);
    }
}
