namespace RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;

public sealed record FaturamentoItem
{
    public required string Pedido { get; init; }
    public required string Cliente { get; init; }
    public required DateTime DataHora { get; init; }
    public required decimal ValorPedido { get; init; }
    public required decimal ValorBase { get; init; }
    public required string Representante { get; init; }
    public required string NomeRepresentante { get; init; }
    public required string NotaFiscal { get; init; }
}
