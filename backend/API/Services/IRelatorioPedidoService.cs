using RedMobilePedidos.API.Models.Responses.Pedidos;

namespace RedMobilePedidos.API.Services;

public interface IRelatorioPedidoService
{
    string GerarRelatorioHtml(PedidoDetalhado pedido);
}
