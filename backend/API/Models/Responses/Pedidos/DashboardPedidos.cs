using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public sealed record DashboardPedidos
{
    public int? TotalPedidosPeriodo { get; init; }
    public int? PedidosAbertosPeriodo { get; init; }
    public decimal? ValorTotalPedidos { get; init; }
    public List<ItemGrafico>? PedidosPorPeriodo { get; init; }
    public List<ItemGrafico>? PedidosPorTipo { get; init; }
}
