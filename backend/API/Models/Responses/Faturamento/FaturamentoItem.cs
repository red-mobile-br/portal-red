namespace RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;

public sealed record FaturamentoItem
{
    public string? NumeroPedido { get; init; }
    public string? NomeCliente { get; init; }
    public DateTime? DataEmissao { get; init; }
    public decimal? ValorBruto { get; init; }
    public decimal? ValorBase { get; init; }
    public string? IdRepresentante { get; init; }
    public string? NomeRepresentante { get; init; }
    public string? NumeroNota { get; init; }
}
