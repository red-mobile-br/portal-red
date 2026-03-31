using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Metas;

namespace RedMobilePedidos.API.Controllers;

[Route("api/meta")]
public class MetaController(IHttpClientFactory httpClientFactory, ILogger<MetaController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpGet]
    public async Task<IEnumerable<Meta>> ObterMetasVigentes([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var hoje = DateTime.Today;
        var dataInicio = queryObject.De ?? new DateTime(hoje.Year, hoje.Month, 1);
        var dataFim = queryObject.Ate ?? new DateTime(dataInicio.Year, dataInicio.Month, DateTime.DaysInMonth(dataInicio.Year, dataInicio.Month));

        var filtroAtualizado = queryObject with { De = dataInicio, Ate = dataFim };

        var url = ConstruirUrlMetasVigentes(UsuarioLogado, filtroAtualizado);
        var resultado = await ObterAsync<ListaMetas>(url, cancellationToken);
        return resultado.Metas;
    }

    [HttpGet("historico")]
    public async Task<IEnumerable<Meta>> ObterMetas([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlMetas(UsuarioLogado, queryObject);
        var resultado = await ObterAsync<ListaMetas>(url, cancellationToken);
        return resultado.Metas;
    }

    private static string ConstruirUrlMetasVigentes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Meta") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private static string ConstruirUrlMetas(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Meta") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));
}
