using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Controllers;

[Route("api/planoPagamento")]
public class PlanoPagamentoController(IHttpClientFactory httpClientFactory, IOptions<ProtheusSettings> protheusOptions, ILogger<PlanoPagamentoController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
{
    [HttpGet]
    public async Task<IEnumerable<PlanoPagamento>> ObterTodos(CancellationToken cancellationToken)
    {
        var url = ConstruirUrlPlanosPagamento();
        var resultado = await ObterAsync<RespostaPaginada<PlanoPagamento>>(url, cancellationToken);
        return resultado.Dados;
    }

    private string ConstruirUrlPlanosPagamento() => string.Join('/', CaminhoApiPadrao, "PlanoPagamento");
}
