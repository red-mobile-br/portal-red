using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Clientes;
using RedMobilePedidos.API.Models.Responses.Comissao;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Models.Responses.Dashboard;

namespace RedMobilePedidos.API.Controllers;

[Route("api/dashboard")]
public class DashboardController(IHttpClientFactory httpClientFactory, ILogger<DashboardController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpGet]
    public async Task<DashboardGeralDto> ObterDashboard([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var urlDashboardPedidos = ConstruirUrlDashboardPedidos(UsuarioLogado, queryObject);
        var urlDashboardComissoes = ConstruirUrlDashboardComissoes(UsuarioLogado, queryObject with { Status = "0" });
        var urlDashboardClientes = ConstruirUrlDashboardClientes(UsuarioLogado, queryObject);

        var dashboardPedidos = await ObterAsync<DashboardPedidos>(urlDashboardPedidos, cancellationToken);
        var dashboardComissoes = await ObterAsync<DashboardComissoes>(urlDashboardComissoes, cancellationToken);
        var dashboardClientes = await ObterAsync<DashboardClientes>(urlDashboardClientes, cancellationToken);

        var resultado = new DashboardGeralDto
        {
            ClientesPorEstado = dashboardClientes.ClientesPorEstado,
            ComissoesPeriodo = dashboardComissoes.ComissoesPeriodo,
            TotalComissaoPeriodo = dashboardComissoes.TotalComissaoPeriodo,
            TotalPedidosPeriodo = dashboardPedidos.TotalPedidosPeriodo,
            PedidosAbertosPeriodo = dashboardPedidos.PedidosAbertosPeriodo
        };

        return resultado;
    }

    private static string ConstruirUrlDashboardPedidos(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Pedido")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private static string ConstruirUrlDashboardComissoes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Comissao")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private static string ConstruirUrlDashboardClientes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Cliente")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
