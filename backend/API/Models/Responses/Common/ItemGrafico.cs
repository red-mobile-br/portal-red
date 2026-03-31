namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record ItemGrafico
{
    public required string Rotulo { get; init; }
    public required List<SerieGrafico> Series { get; init; }
}

public sealed record SerieGrafico
{
    public required string Nome { get; init; }
    public required double Valor { get; init; }
}