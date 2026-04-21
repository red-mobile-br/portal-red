namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record ClientesPorEstado
{
    public string? Uf { get; init; }
    public int? QuantidadeClientes { get; init; }
}
