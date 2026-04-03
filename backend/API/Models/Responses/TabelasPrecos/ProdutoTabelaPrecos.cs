namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record ProdutoTabelaPrecos
{
    public string? ReferenciaComercial { get; init; }
    public string? Descricao { get; init; }
    public string? Imagem { get; init; }
    public ComissaoTabelaPrecos? ComissaoPorcento3 { get; init; }
    public ComissaoTabelaPrecos? ComissaoPorcento5 { get; init; }
    public decimal? PrecoMercado { get; init; }
    public decimal? PercentualIPI { get; init; }
}
