namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record MelhorCliente
{
    public string? NomeCliente { get; init; }
    public double? ValorTotal { get; init; }
}
