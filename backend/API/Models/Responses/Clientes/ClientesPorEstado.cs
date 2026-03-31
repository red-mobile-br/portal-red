namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record ClientesPorEstado
{
    public required string Estado { get; init; }
    public required int QuantidadeClientes { get; init; }
}
