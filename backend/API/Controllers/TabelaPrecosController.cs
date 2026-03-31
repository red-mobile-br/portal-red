using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Responses.TabelaPrecoss;

namespace RedMobilePedidos.API.Controllers;

[Route("api/tabelaPrecos")]
public class TabelaPrecosController(IHttpClientFactory httpClientFactory, ILogger<TabelaPrecosController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpGet("{estado}")]
    public async Task<TabelaPrecos> ObterTabelaPrecos([FromRoute] string estado, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlTabelaPrecos(estado);
        return await ObterAsync<TabelaPrecos>(url, cancellationToken);
    }

    private static string ConstruirUrlTabelaPrecos(string estado)
    {
        var dict = new NameValueCollection
        {
            { "UF", estado.ToUpper() }
        };
        return string.Join('/', CaminhoApiPadrao, "TabelaPrecos") + ConstruirQueryString(dict);
    }
}
