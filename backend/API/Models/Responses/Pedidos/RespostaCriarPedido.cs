using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public sealed record RespostaCriarPedido : RespostaPadrao
{
    public required string Id { get; init; }
}
