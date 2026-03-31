namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record RespostaPaginada<T>
{
    public int NumeroPagina { get; init; } = 1;
    public int TotalPaginas { get; init; } = 1;
    public IEnumerable<T> Dados { get; init; } = new List<T>();
}
