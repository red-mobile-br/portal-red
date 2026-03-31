using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Representantes;

namespace RedMobilePedidos.API.Controllers;

[Route("api/gerentes")]
public class GerentesController(IHttpClientFactory httpClientFactory, ILogger<GerentesController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpGet]
    public async Task<ActionResult<RespostaPaginada<RepresentanteModel>>> ObterGerentes([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ObterUrlGerentes(UsuarioLogado, queryObject);
        return await ObterAsync<RespostaPaginada<RepresentanteModel>>(url, cancellationToken);
    }

    private static string ObterUrlGerentes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Gerentes") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
