namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record ItemGrafico
{
    public string? Rotulo { get; init; }
    public List<SerieGrafico>? Series { get; init; }
}

public sealed record SerieGrafico
{
    public string? Nome { get; init; }
    public double? Valor { get; init; }
}
