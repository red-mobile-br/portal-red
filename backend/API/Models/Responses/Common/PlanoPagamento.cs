namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record PlanoPagamento
{
    public required string Codigo { get; init; }
    public required string Descricao { get; init; }
}
