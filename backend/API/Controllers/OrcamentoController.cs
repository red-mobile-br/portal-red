using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Models.Responses.Orcamentos;

namespace RedMobilePedidos.API.Controllers;

/// <summary>
/// Controller para gerenciar orçamentos.
/// Orçamentos podem ser convertidos em pedidos ao serem aprovados.
/// </summary>
[Route("api/orcamento")]
public class OrcamentoController(IHttpClientFactory httpClientFactory, IOptions<ProtheusSettings> protheusOptions, ILogger<OrcamentoController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
{
    private const string Recurso = "Orcamento";

    [HttpGet]
    public async Task<RespostaPaginada<OrcamentoListaItem>> OrcamentoDetalhados([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        return await ObterAsync<RespostaPaginada<OrcamentoListaItem>>(ConstruirUrlLista(Recurso, queryObject), cancellationToken);
    }

    [HttpGet("{numero}")]
    public async Task<ActionResult<OrcamentoDetalhado>> OrcamentoDetalhadoPorNumero([FromRoute] string numero, CancellationToken cancellationToken)
    {
        var orcamento = await ObterAsync<OrcamentoDetalhado>(ConstruirUrlRecurso(Recurso, numero), cancellationToken);
        return orcamento != null ? Ok(orcamento) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> CriarOrcamento([FromBody] CriarPedido orcamento, CancellationToken cancellationToken)
    {
        var orcamentoCriado = await PublicarAsync<RespostaCriarPedido>(orcamento, ConstruirUrlLista(Recurso), cancellationToken);
        return CreatedAtAction(nameof(OrcamentoDetalhadoPorNumero), new { numero = orcamentoCriado.Id }, orcamentoCriado);
    }

    [HttpPut("{numero}")]
    public async Task<RespostaPadrao> AtualizarOrcamento([FromRoute] string numero, [FromBody] CriarPedido orcamento, CancellationToken cancellationToken)
    {
        return await AtualizarAsync(orcamento, ConstruirUrlRecurso(Recurso, numero), cancellationToken);
    }

    [HttpDelete("{numero}")]
    public async Task<ActionResult> ExcluirOrcamento([FromRoute] string numero, CancellationToken cancellationToken)
    {
        await ExcluirAsync(ConstruirUrlRecurso(Recurso, numero), cancellationToken);
        return NoContent();
    }

    [HttpPost("{numero}/aprovar")]
    public async Task<RespostaPadrao> AprovarOrcamento([FromRoute] string numero, CancellationToken cancellationToken)
    {
        return await PublicarAsync<RespostaPadrao>(null, ConstruirUrlAcao(Recurso, numero, "aprovar"), cancellationToken);
    }

    [HttpPost("{numero}/recusar")]
    public async Task<RespostaPadrao> RecusarOrcamento([FromRoute] string numero, CancellationToken cancellationToken)
    {
        return await PublicarAsync<RespostaPadrao>(null, ConstruirUrlAcao(Recurso, numero, "recusar"), cancellationToken);
    }

    [HttpGet("dashboard")]
    public async Task<DashboardPedidos> OrcamentoDetalhadosDashboard([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        return await ObterAsync<DashboardPedidos>(ConstruirUrlDashboard(Recurso, queryObject), cancellationToken);
    }
}
