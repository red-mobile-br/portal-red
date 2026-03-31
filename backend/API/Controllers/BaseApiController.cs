using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RedMobilePedidos.API.Exceptions;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Settings;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Web;
using RedMobilePedidos.API.Models.Responses.Clientes;
using RedMobilePedidos.API.Models.Responses.Comissao;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Models.Responses.Titulos;

namespace RedMobilePedidos.API.Controllers;

[ApiController]
public abstract class BaseApiController(IHttpClientFactory httpClientFactory, ILogger logger) : ControllerBase
{
    private static readonly AuthenticationHeaderValue CabecalhoAuth = new(nameof(AuthenticationSchemes.Basic),
        Convert.ToBase64String("red-api:!ok$$L8GRb"u8.ToArray()));

    protected const string CaminhoApiPadrao = "http://187.45.183.238:8090/rest";
    private const string FormatoDataProtheus = "yyyyMMdd";

    protected string UsuarioLogado => ObterIdentificador();

    protected ActionResult ResultadoNaoAutorizado(string mensagem)
    {
        var modeloErro = new ModeloErro
        {
            Codigo = StatusCodes.Status401Unauthorized,
            Mensagem = mensagem
        };

        return Unauthorized(modeloErro);
    }

    private string ObterIdentificador()
    {
        var id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        return id ?? throw new UsuarioNaoEncontradoException("Usuário não encontrado. Efetue o login novamente.");
    }

    #region Operações HTTP

    protected async Task<T> ObterAsync<T>(string url, CancellationToken cancellationToken)
    {
        try
        {
            var resposta = await EnviarRequisicaoHttp(url, HttpMethod.Get, null, cancellationToken);
            return JsonSerializer.Deserialize<T>(resposta, JsonOpcoes.Padrao)!;
        }
        catch (FalhaNaRequisicaoException ex)
        {
            var tipo = typeof(T);
            if (!tipo.IsGenericType)
            {
                throw;
            }

            var tipoGenerico = tipo.GetGenericArguments()[0];
            var codigoErroNaoEncontrado = ObterCodigoErroNaoEncontrado(tipoGenerico);
            var erro = JsonSerializer.Deserialize<ModeloErro>(ex.Content, JsonOpcoes.Padrao)!;

            if (erro.Codigo != codigoErroNaoEncontrado)
            {
                throw;
            }

            return default!;
        }
        catch (JsonException ex)
        {
            var erro = $"Houve um erro ao buscar os dados no Protheus. Não foi possível ler os dados da resposta: {ex.Message}";
            throw new Exception(erro);
        }
    }

    protected async Task<T> PublicarAsync<T>(object? corpo, string url, CancellationToken cancellationToken)
    {
        var resposta = await EnviarRequisicaoHttp(url, HttpMethod.Post, corpo, cancellationToken);

        // Sempre verifica se a resposta indica um erro (Protheus retorna HTTP 200 mesmo para erros)
        var respostaPadrao = JsonSerializer.Deserialize<RespostaPadrao>(resposta, JsonOpcoes.Padrao);
        return respostaPadrao is { Sucesso: false, Detalhes: not null }
            ? throw new Exception(respostaPadrao.Detalhes)
            : JsonSerializer.Deserialize<T>(resposta, JsonOpcoes.Padrao)!;
    }

    protected async Task<RespostaPadrao> AtualizarAsync(object corpo, string url, CancellationToken cancellationToken)
    {
        var resposta = await EnviarRequisicaoHttp(url, HttpMethod.Put, corpo, cancellationToken);
        var resultado = JsonSerializer.Deserialize<RespostaPadrao>(resposta, JsonOpcoes.Padrao)!;
        return resultado.Sucesso
            ? resultado
            : throw new Exception(resultado.Detalhes);
    }

    protected async Task<RespostaPadrao> ExcluirAsync(string url, CancellationToken cancellationToken)
    {
        var resposta = await EnviarRequisicaoHttp(url, HttpMethod.Delete, null, cancellationToken);
        var resultado = JsonSerializer.Deserialize<RespostaPadrao>(resposta, JsonOpcoes.Padrao)!;
        return resultado.Sucesso
            ? resultado
            : throw new Exception(resultado.Detalhes);
    }

    private async Task<string> EnviarRequisicaoHttp(string url, HttpMethod metodo, object? corpo, CancellationToken cancellationToken)
    {
        var requisicao = new HttpRequestMessage(metodo, url);
        requisicao.Headers.Authorization = CabecalhoAuth;

        if (corpo != null)
        {
            var corpoSerializado = JsonSerializer.Serialize(corpo, JsonOpcoes.Padrao);
            requisicao.Content = new StringContent(corpoSerializado, Encoding.UTF8, "application/json");
        }

        var cronometro = Stopwatch.StartNew();
        var client = httpClientFactory.CreateClient();
        var resposta = await client.SendAsync(requisicao, cancellationToken);
        var conteudo = await resposta.Content.ReadAsStringAsync(cancellationToken);
        cronometro.Stop();

        if (!resposta.IsSuccessStatusCode)
        {
            logger.LogWarning("Protheus {Metodo} {Url} falhou com {CodigoStatus} em {Duracao}ms",
                metodo, url, (int)resposta.StatusCode, cronometro.ElapsedMilliseconds);
        }

        return resposta.IsSuccessStatusCode
            ? conteudo
            : throw new FalhaNaRequisicaoException(conteudo);
    }

    private static int ObterCodigoErroNaoEncontrado(Type tipo)
    {
        return tipo.Name switch
        {
            nameof(ClienteListaItem) or nameof(ComissaoListaItem) => 1,
            nameof(TituloListaItem) or nameof(PedidoListaItem) => 400,
            _ => 0,
        };
    }

    #endregion

    #region Métodos de Construção de URL

    /// <summary>
    /// Constrói uma URL para listagem de recursos com parâmetros de consulta.
    /// Exemplo: /rest/Pedido?UsuarioLogado=xxx&Pagina=1
    /// </summary>
    protected string ConstruirUrlLista(string recurso, FiltroPadraoQuery? filtro = null)
        => $"{CaminhoApiPadrao}/{recurso}{ConstruirQueryString(ObterParametrosQuery(filtro))}";

    /// <summary>
    /// Constrói uma URL para um recurso específico por ID.
    /// Exemplo: /rest/Pedido/123?UsuarioLogado=xxx
    /// </summary>
    protected string ConstruirUrlRecurso(string recurso, string id)
        => $"{CaminhoApiPadrao}/{recurso}/{id}{ConstruirQueryString(ObterParametrosQuery(null))}";

    /// <summary>
    /// Constrói uma URL para uma ação em um recurso.
    /// Exemplo: /rest/Pedido/123/aprovar?UsuarioLogado=xxx
    /// </summary>
    protected string ConstruirUrlAcao(string recurso, string id, string acao)
        => $"{CaminhoApiPadrao}/{recurso}/{id}/{acao}{ConstruirQueryString(ObterParametrosQuery(null))}";

    /// <summary>
    /// Constrói uma URL para o dashboard de um recurso.
    /// Exemplo: /rest/Dashboard/Order?UsuarioLogado=xxx
    /// </summary>
    protected string ConstruirUrlDashboard(string recurso, FiltroPadraoQuery? filtro = null)
        => $"{CaminhoApiPadrao}/Dashboard/{recurso}{ConstruirQueryString(ObterParametrosQuery(filtro))}";

    /// <summary>
    /// Retorna os parâmetros base de consulta incluindo UsuarioLogado.
    /// </summary>
    private NameValueCollection ObterParametrosQuery(FiltroPadraoQuery? filtro)
        => ObterDicionarioQueryString(UsuarioLogado, filtro);

    // Métodos legados mantidos para compatibilidade
    protected static string ObterCaminhoDashboard(string urlBase)
        => urlBase.Insert(urlBase.LastIndexOf('/'), "/Dashboard");

    protected static NameValueCollection ObterDicionarioQueryString(string? usuarioLogado, FiltroPadraoQuery? filtro)
    {
        var dict = new NameValueCollection();

        if (!string.IsNullOrEmpty(usuarioLogado))
        {
            dict.Add("UsuarioLogado", usuarioLogado);
        }

        if (filtro?.De != null)
        {
            dict.Add("DataDe", filtro.De.Value.ToString(FormatoDataProtheus));
        }

        if (filtro?.Ate != null)
        {
            dict.Add("DataAte", filtro.Ate.Value.ToString(FormatoDataProtheus));
        }

        if (filtro?.Pagina != null)
        {
            dict.Add("Pagina", filtro.Pagina.ToString());
        }

        if (filtro?.Tamanho != null)
        {
            dict.Add("Tamanho", filtro.Tamanho.ToString());
        }

        if (!string.IsNullOrEmpty(filtro?.Direcao))
        {
            dict.Add("Direcao", filtro.Direcao);
        }

        if (!string.IsNullOrEmpty(filtro?.OrdenarPor))
        {
            dict.Add("OrdenarPor", filtro.OrdenarPor);
        }

        if (!string.IsNullOrEmpty(filtro?.Busca))
        {
            dict.Add("Busca", filtro.Busca);
        }

        if (!string.IsNullOrEmpty(filtro?.Status))
        {
            dict.Add("Status", filtro.Status);
        }

        if (!string.IsNullOrEmpty(filtro?.Cnpj))
        {
            dict.Add("Cnpj", filtro.Cnpj);
        }

        if (!string.IsNullOrEmpty(filtro?.FiltroUsuario))
        {
            dict.Add("FiltroUsuario", filtro.FiltroUsuario);
        }

        if (!string.IsNullOrEmpty(filtro?.IdRepresentante))
        {
            dict.Add("IdRepresentante", filtro.IdRepresentante);
        }

        if (!string.IsNullOrEmpty(filtro?.IdGerente))
        {
            dict.Add("IdGerente", filtro.IdGerente);
        }

        if (!string.IsNullOrEmpty(filtro?.IdCliente))
        {
            dict.Add("IdCliente", filtro.IdCliente);
        }

        return dict;
    }

    protected static string ConstruirQueryString(NameValueCollection args)
    {
        var resultado = (
            from chave in args.AllKeys
            let valor = args.Get(chave)
            select $"{HttpUtility.UrlEncode(chave)}={HttpUtility.UrlEncode(valor)}")
            .ToList();

        return "?" + string.Join("&", resultado);
    }

    #endregion
}
