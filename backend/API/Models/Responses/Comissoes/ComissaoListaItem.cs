namespace RedMobilePedidos.API.Models.Responses.Comissao;

public sealed record ComissaoListaItem
{
    public string? NumeroPedido { get; init; }
    public string? NomeCliente { get; init; }
    public string? NumeroTitulo { get; init; }
    public int? Parcela { get; init; }
    public DateTime? DataVencimento { get; init; }
    public DateTime? DataBaixa { get; init; }
    public decimal? ValorTitulo { get; init; }
    public decimal? ValorBase { get; init; }
    public decimal? PercentualComissao { get; init; }
    public decimal? ValorComissao { get; init; }
    public string? IdRepresentante { get; init; }
    public string? NomeRepresentante { get; init; }
}
