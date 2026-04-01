using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Comissao;

namespace RedMobilePedidos.API.Controllers;

[Route("api/comissao")]
public class ComissaoController(IHttpClientFactory httpClientFactory, IOptions<ProtheusSettings> protheusOptions, ILogger<ComissaoController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
{
    private static readonly string[] StatusValidos = ["0", "1", "2"];

    [HttpGet]
    public async Task<RespostaPaginada<ComissaoListaItem>> ObterComissoes([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(queryObject.Status) || !StatusValidos.Contains(queryObject.Status))
        {
            throw new Exception("Status inválido. Os valores válidos são 1 e 2");
        }

        var url = ConstruirUrlComissoes(UsuarioLogado, queryObject);
        return await ObterAsync<RespostaPaginada<ComissaoListaItem>>(url, cancellationToken);
    }

    [HttpGet("dashboard")]
    public async Task<DashboardComissoes> ObterDashboardComissoes([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(queryObject.Status) || !StatusValidos.Contains(queryObject.Status))
        {
            throw new Exception("Status inválido. Os valores válidos são 0, 1 e 2");
        }

        var url = ConstruirUrlDashboardComissoes(UsuarioLogado, queryObject);
        return await ObterAsync<DashboardComissoes>(url, cancellationToken);
    }

    private string ConstruirUrlComissoes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Comissao") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private string ConstruirUrlDashboardComissoes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Comissao")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
