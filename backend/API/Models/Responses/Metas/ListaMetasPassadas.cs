namespace RedMobilePedidos.API.Models.Responses.Metas;

public sealed record ListaMetasPassadas
{
    public IEnumerable<Meta>? MetasPassadas { get; init; }
}
