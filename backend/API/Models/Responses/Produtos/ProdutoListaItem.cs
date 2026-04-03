namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record ProdutoListaItem
{
    public string? Id { get; init; }
    public string? Descricao { get; init; }
    public decimal? ValorUnitario { get; init; }
}
