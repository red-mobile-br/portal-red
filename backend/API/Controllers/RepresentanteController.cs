using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Representantes;

namespace RedMobilePedidos.API.Controllers;

[Route("api/representantes")]
public class RepresentantesController(IHttpClientFactory httpClientFactory, IOptions<ProtheusSettings> protheusOptions, ILogger<RepresentantesController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
{
    [HttpGet]
    public async Task<ActionResult<RespostaPaginada<RepresentanteModel>>> ObterRepresentantes([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ObterUrlRepresentantes(UsuarioLogado, queryObject);
        return await ObterAsync<RespostaPaginada<RepresentanteModel>>(url, cancellationToken);
    }

    private string ObterUrlRepresentantes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Representantes") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
