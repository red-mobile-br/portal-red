namespace RedMobilePedidos.API.Models.Responses.Titulos;

public sealed record TituloListaItem
{
    public string? NumeroTitulo { get; init; }
    public string? NomeCliente { get; init; }
    public int? Parcela { get; init; }
    public string? Cnpj { get; init; }
    public string? NumeroPedido { get; init; }
    public decimal? ValorTitulo { get; init; }
    public int? Status { get; init; }
    public DateTime? DataVencimento { get; init; }
    public DateTime? DataPagamento { get; init; }
    public DateTime? VencimentoOriginal { get; init; }
    public string? NomeRepresentante { get; init; }
    public string? IdRepresentante { get; init; }
}
