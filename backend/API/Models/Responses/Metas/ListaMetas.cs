namespace RedMobilePedidos.API.Models.Responses.Metas;

public sealed record ListaMetas
{
    public required IEnumerable<Meta> Metas { get; init; }
}
