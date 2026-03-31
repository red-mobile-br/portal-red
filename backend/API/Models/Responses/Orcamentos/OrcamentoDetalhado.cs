using RedMobilePedidos.API.Models.Responses.Pedidos;

namespace RedMobilePedidos.API.Models.Responses.Orcamentos;

public sealed record OrcamentoDetalhado : PedidoDetalhado
{
    public required DateTime DataImplementacao { get; init; }
    public required DateTime DataCancelamento { get; init; }
}
