namespace RedMobilePedidos.API.Models.Responses.Metas;

public sealed record ListaMetas
{
    public IEnumerable<Meta>? Metas { get; init; }
}
