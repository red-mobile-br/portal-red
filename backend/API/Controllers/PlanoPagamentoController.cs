using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Controllers;

[Route("api/planoPagamento")]
public class PlanoPagamentoController(IHttpClientFactory httpClientFactory, ILogger<PlanoPagamentoController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpGet]
    public async Task<IEnumerable<PlanoPagamento>> ObterTodos(CancellationToken cancellationToken)
    {
        var url = ConstruirUrlPlanosPagamento();
        var resultado = await ObterAsync<RespostaPaginada<PlanoPagamento>>(url, cancellationToken);
        return resultado.Dados;
    }

    private static string ConstruirUrlPlanosPagamento() => string.Join('/', CaminhoApiPadrao, "PlanoPagamento");
}
