namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record ProdutoTabelaPrecos
{
    public string? Referencia { get; init; }
    public string? Descricao { get; init; }
    public string? Imagem { get; init; }
    public decimal? PrecoTabela { get; init; }
    public decimal? PercentualIPI { get; init; }
}
