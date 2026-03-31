namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record MelhorCliente
{
    public required string NomeCliente { get; init; }
    public required double ValorTotal { get; init; }
}
