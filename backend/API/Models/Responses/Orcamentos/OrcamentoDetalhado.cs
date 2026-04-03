using RedMobilePedidos.API.Models.Responses.Pedidos;

namespace RedMobilePedidos.API.Models.Responses.Orcamentos;

public sealed record OrcamentoDetalhado : PedidoDetalhado
{
    public DateTime? DataImplementacao { get; init; }
    public DateTime? DataCancelamento { get; init; }
}
