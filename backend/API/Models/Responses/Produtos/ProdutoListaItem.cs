namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record ProdutoListaItem
{
    public required string Id { get; init; }
    public required string Descricao { get; init; }
    public required decimal ValorUnitario { get; init; }
}
