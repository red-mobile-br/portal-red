using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Clientes;

namespace RedMobilePedidos.API.Models.Responses.Dashboard;

public sealed record DashboardGeralDto
{
    public required int TotalPedidosPeriodo { get; init; }
    public required decimal TotalComissaoPeriodo { get; init; }
    public required int PedidosAbertosPeriodo { get; init; }
    public required List<ItemGrafico> ComissoesPeriodo { get; init; }
    public required List<ClientesPorEstado> ClientesPorEstado { get; init; }
}
