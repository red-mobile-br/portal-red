using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Email;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Services;

namespace RedMobilePedidos.API.Controllers;

/// <summary>
/// Controller para gerenciar pedidos.
/// </summary>
[Route("api/pedido")]
public class PedidoController(
    IHttpClientFactory httpClientFactory,
    IEmailService emailService,
    IPdfService pdfService,
    IRelatorioPedidoService servicoRelatorioPedido,
    IEmailTemplateService emailTemplateService,
    IOptions<ProtheusSettings> protheusOptions,
    ILogger<PedidoController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
{
    private const string Recurso = "Pedido";

    [HttpGet]
    public async Task<RespostaPaginada<PedidoListaItem>> PedidoDetalhados([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        return await ObterAsync<RespostaPaginada<PedidoListaItem>>(ConstruirUrlLista(Recurso, queryObject), cancellationToken);
    }

    [HttpGet("{numero}")]
    public async Task<ActionResult<PedidoDetalhado>> PedidoDetalhadoPorNumero([FromRoute] string numero, CancellationToken cancellationToken)
    {
        var pedido = await ObterAsync<PedidoDetalhado>(ConstruirUrlRecurso(Recurso, numero), cancellationToken);
        return pedido != null ? Ok(pedido) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> CriarPedido([FromBody] CriarPedido pedido, CancellationToken cancellationToken)
    {
        var pedidoCriado = await PublicarAsync<RespostaCriarPedido>(pedido, ConstruirUrlLista(Recurso), cancellationToken);
        return CreatedAtAction(nameof(PedidoDetalhadoPorNumero), new { numero = pedidoCriado.Id }, pedidoCriado);
    }

    [HttpPut("{numero}")]
    public async Task<RespostaPadrao> AtualizarPedido([FromRoute] string numero, [FromBody] CriarPedido pedido, CancellationToken cancellationToken)
    {
        return await AtualizarAsync(pedido, ConstruirUrlRecurso(Recurso, numero), cancellationToken);
    }

    [HttpDelete("{numero}")]
    public async Task<ActionResult> ExcluirPedido([FromRoute] string numero, CancellationToken cancellationToken)
    {
        await ExcluirAsync(ConstruirUrlRecurso(Recurso, numero), cancellationToken);
        return NoContent();
    }

    [HttpPost("{numero}/aprovar")]
    public async Task<RespostaPadrao> AprovarPedido([FromRoute] string numero, CancellationToken cancellationToken)
    {
        return await PublicarAsync<RespostaPadrao>(null, ConstruirUrlAcao(Recurso, numero, "aprovar"), cancellationToken);
    }

    [HttpPost("{numero}/recusar")]
    public async Task<RespostaPadrao> RecusarPedido([FromRoute] string numero, CancellationToken cancellationToken)
    {
        return await PublicarAsync<RespostaPadrao>(null, ConstruirUrlAcao(Recurso, numero, "recusar"), cancellationToken);
    }

    [HttpPost("{numero}/email")]
    public async Task<ActionResult> EnviarEmail([FromRoute] string numero, [FromBody] EmailModel model, CancellationToken cancellationToken)
    {
        var pedido = await ObterAsync<PedidoDetalhado>(ConstruirUrlRecurso(Recurso, numero), cancellationToken);
        if (pedido == null)
        {
            return NotFound(new { mensagem = "Pedido não encontrado" });
        }

        var html = servicoRelatorioPedido.GerarRelatorioHtml(pedido);
        var pdfBytes = await pdfService.GerarPdfDeHtmlAsync(html);

        var assunto = $"Pedido #{pedido.Id} - {pedido.Cliente.RazaoSocial}";
        var corpoEmail = GerarCorpoEmail(pedido);

        await emailService.EnviarEmailAsync(
            model.Destinatario,
            assunto,
            corpoEmail,
            pdfBytes,
            $"Pedido_{pedido.Id}_{pedido.DataEmissao:yyyyMMdd}.pdf"
        );

        return Ok(new { mensagem = "Email enviado com sucesso" });
    }

    private string GerarCorpoEmail(PedidoDetalhado pedido)
    {
        var modeloEmail = new EmailPedidoModel
        {
            IdPedido = pedido.Id,
            RazaoSocialCliente = pedido.Cliente.RazaoSocial
        };

        return emailTemplateService.RenderizarTemplate("EmailPedido", modeloEmail);
    }

    [HttpGet("{numero}/pdf")]
    public async Task<ActionResult> BaixarPdf([FromRoute] string numero, CancellationToken cancellationToken)
    {
        var pedido = await ObterAsync<PedidoDetalhado>(ConstruirUrlRecurso(Recurso, numero), cancellationToken);
        if (pedido == null)
        {
            return NotFound(new { mensagem = "Pedido não encontrado" });
        }

        var html = servicoRelatorioPedido.GerarRelatorioHtml(pedido);
        var pdfBytes = await pdfService.GerarPdfDeHtmlAsync(html);

        return File(pdfBytes, "application/pdf", $"Pedido_{pedido.Id}_{pedido.DataEmissao:yyyyMMdd}.pdf");
    }

    [HttpGet("{numero}/notaFiscal/{id}")]
    public async Task<DadosBase64> PedidoDetalhadoNotaFiscal([FromRoute] string numero, [FromRoute] string id, CancellationToken cancellationToken)
    {
        return await ObterAsync<DadosBase64>(ConstruirUrlAcao(Recurso, numero, $"notaFiscal/{id}"), cancellationToken);
    }

    [HttpGet("{numero}/boleto/{id}")]
    public async Task<DadosBase64> PedidoDetalhadoBoleto([FromRoute] string numero, [FromRoute] string id, CancellationToken cancellationToken)
    {
        return await ObterAsync<DadosBase64>(ConstruirUrlAcao(Recurso, numero, $"boleto/{id}"), cancellationToken);
    }

    [HttpGet("dashboard")]
    public async Task<DashboardPedidos> PedidoDetalhadosDashboard([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        return await ObterAsync<DashboardPedidos>(ConstruirUrlDashboard(Recurso, queryObject), cancellationToken);
    }
}
