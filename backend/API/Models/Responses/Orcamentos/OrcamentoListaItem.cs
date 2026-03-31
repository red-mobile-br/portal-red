using RedMobilePedidos.API.Models.Responses.Pedidos;

namespace RedMobilePedidos.API.Models.Responses.Orcamentos;

public sealed record OrcamentoListaItem : PedidoListaItem
{
    public required string IdPedido { get; init; }
    public required DateTime DataImplementacao { get; init; }
    public required DateTime DataCancelamento { get; init; }
}
