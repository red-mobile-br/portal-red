namespace RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

public sealed record ProdutoTabelaPrecos
{
    public required string ReferenciaComercial { get; init; }
    public required string Descricao { get; init; }
    public required string Imagem { get; init; }
    public required ComissaoTabelaPrecos ComissaoPorcento3 { get; init; }
    public required ComissaoTabelaPrecos ComissaoPorcento5 { get; init; }
    public required decimal PrecoMercado { get; init; }
    public required decimal PercentualIPI { get; init; }
}
