using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Comissao;

public sealed record DashboardComissoes
{
    public required decimal TotalComissaoPeriodo { get; init; }
    public required decimal PercentualComissaoPeriodo { get; init; }
    public required List<ItemGrafico> ComissoesPeriodo { get; init; }
    public required List<ItemGrafico> MaioresComissoesPeriodo { get; init; }
}
