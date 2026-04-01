using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Clientes;
using RedMobilePedidos.API.Models.Responses.Comissao;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Models.Responses.Dashboard;

namespace RedMobilePedidos.API.Controllers;

[Route("api/dashboard")]
public class DashboardController(IHttpClientFactory httpClientFactory, IOptions<ProtheusSettings> protheusOptions, ILogger<DashboardController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
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

    private string ConstruirUrlDashboardPedidos(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Pedido")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private string ConstruirUrlDashboardComissoes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Comissao")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private string ConstruirUrlDashboardClientes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Cliente")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
