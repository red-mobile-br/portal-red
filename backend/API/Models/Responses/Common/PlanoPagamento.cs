namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record PlanoPagamento
{
    public string? Codigo { get; init; }
    public string? Descricao { get; init; }
}
