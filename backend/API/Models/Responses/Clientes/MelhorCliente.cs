namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record MelhorCliente
{
    public string? Nome { get; init; }
    public double? Valor { get; init; }
}
