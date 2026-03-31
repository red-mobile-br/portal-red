using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Titulos;

public sealed record DashboardTitulos
{
    public required decimal ValorRecebido { get; init; }
    public required decimal ValorAReceber { get; init; }
    public required List<ItemGrafico> DesempenhoPeriodo { get; init; }
}
