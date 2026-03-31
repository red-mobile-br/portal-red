namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record TabelaPrecos
{
    public required string Descricao { get; init; }
    public required DateTime DataAtualizacao { get; init; }
    public required string Comentarios { get; init; }
    public required decimal ValorMinimoFreteCif { get; init; }
    public required string Prazo { get; init; }
    public required decimal ICMS { get; init; }
    public required List<ProdutoTabelaPrecos> Produtos { get; init; }
}
