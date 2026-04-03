using RedMobilePedidos.API.Models.Responses.Pedidos;

namespace RedMobilePedidos.API.Models.Responses.Orcamentos;

public sealed record OrcamentoListaItem : PedidoListaItem
{
    public string? IdPedido { get; init; }
    public DateTime? DataImplementacao { get; init; }
    public DateTime? DataCancelamento { get; init; }
}
