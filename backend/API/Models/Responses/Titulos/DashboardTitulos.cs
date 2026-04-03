using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Titulos;

public sealed record DashboardTitulos
{
    public decimal? ValorRecebido { get; init; }
    public decimal? ValorAReceber { get; init; }
    public List<ItemGrafico>? DesempenhoPeriodo { get; init; }
}
