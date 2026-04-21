using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Clientes;

namespace RedMobilePedidos.API.Models.Responses.Dashboard;

public sealed record DashboardGeralDto
{
    public int? TotalPedidosNoPeriodo { get; init; }
    public decimal? TotalComissaoPeriodo { get; init; }
    public int? PedidosAbertosNoPeriodo { get; init; }
    public List<ItemGrafico>? ComissoesPeriodo { get; init; }
    public List<ClientesPorEstado>? ClientesPorEstado { get; init; }
}
