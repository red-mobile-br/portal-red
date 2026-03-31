using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public sealed record DashboardPedidos
{
    public required int TotalPedidosPeriodo { get; init; }
    public required int PedidosAbertosPeriodo { get; init; }
    public required decimal ValorTotalPedidos { get; init; }
    public required List<ItemGrafico> PedidosPorPeriodo { get; init; }
    public required List<ItemGrafico> PedidosPorTipo { get; init; }
}
