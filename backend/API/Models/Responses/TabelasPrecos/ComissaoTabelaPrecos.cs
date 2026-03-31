namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record ComissaoTabelaPrecos
{
    public required decimal PrecoSemIpi { get; init; }
    public required decimal PercentualIPI { get; init; }
    public required decimal PrecoComIpi { get; init; }
    public required int UnidadesPorEmbalagem { get; init; }
}
