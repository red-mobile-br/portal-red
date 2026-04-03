using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record ClienteListaItem
{
    public string? Id { get; init; }
    public string? Cnpj { get; init; }
    public string? Nome { get; init; }
    public string? Cidade { get; init; }
    public string? UF { get; init; }
    public StatusCliente? Status { get; init; }
    public DateTime? UltimaCompra { get; init; }
    public int? DiasSemComprar { get; init; }
    public string? IdRepresentante { get; init; }
    public string? NomeRepresentante { get; init; }
}
