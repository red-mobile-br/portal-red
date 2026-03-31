using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record ClienteListaItem
{
    public required string Id { get; init; }
    public required string Cnpj { get; init; }
    public required string Nome { get; init; }
    public required string Cidade { get; init; }
    public required string UF { get; init; }
    public required StatusCliente Status { get; init; }
    public DateTime? UltimaCompra { get; init; }
    public required int DiasSemComprar { get; init; }
    public required string IdRepresentante { get; init; }
    public required string NomeRepresentante { get; init; }
}
