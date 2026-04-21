using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Comissao;

public sealed record DashboardComissoes
{
    public decimal? TotalComissaoPorPeriodo { get; init; }
    public decimal? PercentualComissaoPorPeriodo { get; init; }
    public List<ItemGrafico>? ComissoesPorPeriodo { get; init; }
    public List<ItemGrafico>? MaiorComissaoPorPeriodo { get; init; }
}
