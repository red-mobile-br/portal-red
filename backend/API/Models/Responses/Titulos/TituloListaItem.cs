namespace RedMobilePedidos.API.Models.Responses.Titulos;

public sealed record TituloListaItem
{
    public string? Titulo { get; init; }
    public string? NomeCliente { get; init; }
    public int? Parcela { get; init; }
    public string? Cnpj { get; init; }
    public string? Pedido { get; init; }
    public decimal? ValorTitulo { get; init; }
    public string? Status { get; init; }
    public DateTime? DataVencimento { get; init; }
    public DateTime? DataPagamento { get; init; }
    public DateTime? DataVencimentoOriginal { get; init; }
    public string? NomeRepresentante { get; init; }
    public string? Representante { get; init; }
}
