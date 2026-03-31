namespace RedMobilePedidos.API.Models.Responses.Titulos;

public sealed record TituloListaItem
{
    public required string Titulo { get; init; }
    public required string NomeCliente { get; init; }
    public required int Parcela { get; init; }
    public required string Cnpj { get; init; }
    public required string Pedido { get; init; }
    public required decimal ValorTitulo { get; init; }
    public required string Status { get; init; }
    public DateTime? DataVencimento { get; init; }
    public DateTime? DataPagamento { get; init; }
    public DateTime? DataVencimentoOriginal { get; init; }
    public required string NomeRepresentante { get; init; }
    public required string Representante { get; init; }
}
