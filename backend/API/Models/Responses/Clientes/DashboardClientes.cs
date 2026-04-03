using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record DashboardClientes
{
    public List<ClientesPorEstado>? ClientesPorEstado { get; init; }
    public List<ItemGrafico>? ClientesPorEstatus { get; init; }
    public List<MelhorCliente>? MelhoresClientesPorPedidos { get; init; }
    public List<MelhorCliente>? MelhoresClientesPorFaturamento { get; init; }
}
