namespace RedMobilePedidos.API.Models.Responses.Comissao;

public sealed record ComissaoListaItem
{
    public string? Pedido { get; init; }
    public string? NomeCliente { get; init; }
    public string? Cnpj { get; init; }
    public string? Titulo { get; init; }
    public int? Parcela { get; init; }
    public DateTime? DataVencimento { get; init; }
    public DateTime? DataBaixa { get; init; }
    public decimal? ValorTitulo { get; init; }
    public decimal? ValorBase { get; init; }
    public decimal? PercentualComissao { get; init; }
    public decimal? ValorComissao { get; init; }
    public string? Representante { get; init; }
    public string? NomeRepresentante { get; init; }
}
