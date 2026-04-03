using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Clientes;

namespace RedMobilePedidos.API.Models.Responses.Dashboard;

public sealed record DashboardGeralDto
{
    public int? TotalPedidosPeriodo { get; init; }
    public decimal? TotalComissaoPeriodo { get; init; }
    public int? PedidosAbertosPeriodo { get; init; }
    public List<ItemGrafico>? ComissoesPeriodo { get; init; }
    public List<ClientesPorEstado>? ClientesPorEstado { get; init; }
}
