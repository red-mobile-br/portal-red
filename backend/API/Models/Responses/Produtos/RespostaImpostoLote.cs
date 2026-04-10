namespace RedMobilePedidos.API.Models.Responses.Produtos;

/// <summary>
/// Resposta do Protheus POST /itens/impostos (WSTaxes.prw → WSRESTFUL ItensImpostos).
/// </summary>
public sealed record RespostaImpostoLote
{
    public List<ImpostoProduto>? Itens { get; init; }
    public decimal? TotalProdutos { get; init; }
    public decimal? TotalIcms { get; init; }
    public decimal? TotalIpi { get; init; }
    public decimal? TotalIcmsSt { get; init; }
    public decimal? TotalNota { get; init; }
    public decimal? PesoLiquido { get; init; }
    public decimal? PesoBruto { get; init; }
    public decimal? MargemPedido { get; init; }
    public int? TotalVolumes { get; init; }
    public int? TotalItens { get; init; }
}
