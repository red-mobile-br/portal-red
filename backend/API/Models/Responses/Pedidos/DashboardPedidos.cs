using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public sealed record DashboardPedidos
{
    public int? TotalPedidosNoPeriodo { get; init; }
    public int? PedidosAbertosNoPeriodo { get; init; }
    public decimal? ValorTotalPedidos { get; init; }
    public List<ItemGrafico>? PedidosPorPeriodo { get; init; }
    public List<ItemGrafico>? PedidosPorTipo { get; init; }
}
