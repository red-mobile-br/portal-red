namespace RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;

public sealed record FaturamentoItem
{
    public string? Pedido { get; init; }
    public string? Cliente { get; init; }
    public DateTime? DataHora { get; init; }
    public decimal? ValorPedido { get; init; }
    public decimal? ValorBase { get; init; }
    public string? Representante { get; init; }
    public string? NomeRepresentante { get; init; }
    public string? NotaFiscal { get; init; }
}
