using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Titulos;

namespace RedMobilePedidos.API.Controllers;

[Route("api/titulo")]
public class TituloController(IHttpClientFactory httpClientFactory, ILogger<TituloController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpGet]
    public async Task<RespostaPaginada<TituloListaItem>> ObterTitulos([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlTitulos(UsuarioLogado, queryObject);
        return await ObterAsync<RespostaPaginada<TituloListaItem>>(url, cancellationToken);
    }

    [HttpGet("dashboard")]
    public async Task<DashboardTitulos> ObterDashboardTitulos([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlDashboardTitulos(UsuarioLogado, queryObject);
        return await ObterAsync<DashboardTitulos>(url, cancellationToken);
    }

    private static string ConstruirUrlTitulos(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Titulo") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private static string ConstruirUrlDashboardTitulos(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Titulo")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
