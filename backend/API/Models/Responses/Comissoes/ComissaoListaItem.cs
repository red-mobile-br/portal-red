namespace RedMobilePedidos.API.Models.Responses.Comissao;

public sealed record ComissaoListaItem
{
    public required string Pedido { get; init; }
    public required string NomeCliente { get; init; }
    public required string Cnpj { get; init; }
    public required string Titulo { get; init; }
    public required int Parcela { get; init; }
    public DateTime? DataVencimento { get; init; }
    public DateTime? DataBaixa { get; init; }
    public required decimal ValorTitulo { get; init; }
    public required decimal ValorBase { get; init; }
    public required decimal PercentualComissao { get; init; }
    public required decimal ValorComissao { get; init; }
    public required string Representante { get; init; }
    public required string NomeRepresentante { get; init; }
}
