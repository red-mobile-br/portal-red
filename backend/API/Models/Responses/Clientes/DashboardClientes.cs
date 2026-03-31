using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record DashboardClientes
{
    public required List<ClientesPorEstado> ClientesPorEstado { get; init; }
    public required List<ItemGrafico> ClientesPorEstatus { get; init; }
    public required List<MelhorCliente> MelhoresClientesPorPedidos { get; init; }
    public required List<MelhorCliente> MelhoresClientesPorFaturamento { get; init; }
}
