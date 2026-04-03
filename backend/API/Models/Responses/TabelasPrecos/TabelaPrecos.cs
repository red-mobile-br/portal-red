namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record TabelaPrecos
{
    public string? Descricao { get; init; }
    public DateTime? DataAtualizacao { get; init; }
    public string? Comentarios { get; init; }
    public decimal? ValorMinimoFreteCif { get; init; }
    public string? Prazo { get; init; }
    public decimal? ICMS { get; init; }
    public List<ProdutoTabelaPrecos>? Produtos { get; init; }
}
