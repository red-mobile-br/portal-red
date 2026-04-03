namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record RespostaImpostoLote
{
    public List<ImpostoProduto>? Itens { get; init; }
    public decimal? TotalProdutos { get; init; }
    public decimal? TotalICMS { get; init; }
    public decimal? TotalIPI { get; init; }
    public decimal? TotalICMSST { get; init; }
    public decimal? TotalNF { get; init; }
    public decimal? PesoLiquido { get; init; }
    public decimal? PesoBruto { get; init; }
    public decimal? MargemPedido { get; init; }
    public int? TotalVolumes { get; init; }
    public int? TotalItens { get; init; }
}
