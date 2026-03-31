namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record Produto
{
    public required string Id { get; init; }
    public required string Descricao { get; init; }
    public required decimal PrecoBase { get; init; }
    public required decimal PercentualIPI { get; init; }
    public required decimal PercentualICMS { get; init; }
    public required decimal ValorICMSST { get; init; }
    public required decimal PesoLiquido { get; init; }
    public required decimal PesoBruto { get; init; }
}
