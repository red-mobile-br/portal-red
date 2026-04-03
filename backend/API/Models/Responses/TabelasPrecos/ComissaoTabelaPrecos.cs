namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record ComissaoTabelaPrecos
{
    public decimal? PrecoSemIpi { get; init; }
    public decimal? PercentualIPI { get; init; }
    public decimal? PrecoComIpi { get; init; }
    public int? UnidadesPorEmbalagem { get; init; }
}
