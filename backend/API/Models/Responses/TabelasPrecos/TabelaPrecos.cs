namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record TabelaPrecos
{
    public string? Descricao { get; init; }
    public DateTime? DataAtualizacao { get; init; }
    public DateTime? DataValidade { get; init; }
    public string? Observacoes { get; init; }
    public decimal? ValorMinimoFreteCif { get; init; }
    public decimal? ICMS { get; init; }
    public List<ProdutoTabelaPrecos>? Produtos { get; init; }
}
