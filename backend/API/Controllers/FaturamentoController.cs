using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;

namespace RedMobilePedidos.API.Controllers;

[Route("api/faturamento")]
public class DashboardFaturamentoController(IHttpClientFactory httpClientFactory, ILogger<DashboardFaturamentoController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpGet]
    public async Task<RespostaPaginada<FaturamentoItem>> ObterFaturamentos([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlFaturamentos(UsuarioLogado, queryObject);
        return await ObterAsync<RespostaPaginada<FaturamentoItem>>(url, cancellationToken);
    }

    [HttpGet("dashboard")]
    public async Task<DashboardFaturamento> ObterDashboardFaturamentos([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlDashboardFaturamentos(UsuarioLogado, queryObject);
        return await ObterAsync<DashboardFaturamento>(url, cancellationToken);
    }

    private static string ConstruirUrlFaturamentos(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Faturamento") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private static string ConstruirUrlDashboardFaturamentos(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Faturamento")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
