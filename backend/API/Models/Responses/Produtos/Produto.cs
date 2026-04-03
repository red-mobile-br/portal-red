namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record Produto
{
    public string? Id { get; init; }
    public string? Descricao { get; init; }
    public decimal? PrecoBase { get; init; }
    public decimal? PercentualIPI { get; init; }
    public decimal? PercentualICMS { get; init; }
    public decimal? ValorICMSST { get; init; }
    public decimal? PesoLiquido { get; init; }
    public decimal? PesoBruto { get; init; }
}
