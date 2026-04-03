using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Comissao;

public sealed record DashboardComissoes
{
    public decimal? TotalComissaoPeriodo { get; init; }
    public decimal? PercentualComissaoPeriodo { get; init; }
    public List<ItemGrafico>? ComissoesPeriodo { get; init; }
    public List<ItemGrafico>? MaioresComissoesPeriodo { get; init; }
}
