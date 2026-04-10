using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Clientes;
using RedMobilePedidos.API.Models.Responses.Produtos;
using RedMobilePedidos.API.Settings;
using System.Text.Json;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using ArquivoInfo = RedMobilePedidos.API.Models.Responses.Common.ArquivoInfo;

namespace RedMobilePedidos.API.Controllers;

[Route("api/cliente")]
public class ClienteController(IHttpClientFactory httpClientFactory, IOptions<ProtheusSettings> protheusOptions, ILogger<ClienteController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
{
    [HttpGet]
    public async Task<RespostaPaginada<ClienteListaItem>> ObterClientes([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlClientesParaPedido(UsuarioLogado, queryObject);
        return await ObterAsync<RespostaPaginada<ClienteListaItem>>(url, cancellationToken);
    }

    [HttpGet("{idCliente}/produtos")]
    public async Task<RespostaPaginada<ProdutoListaItem>> Produtos([FromRoute] string idCliente, [FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlProdutos(idCliente, queryObject);
        return await ObterAsync<RespostaPaginada<ProdutoListaItem>>(url, cancellationToken);
    }

    [HttpGet("{idCliente}/produto/{codigoProduto}")]
    public async Task<ActionResult<Produto>> Produto([FromRoute] string idCliente, [FromRoute] string codigoProduto, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlProduto(idCliente, codigoProduto);
        var produto = await ObterAsync<Produto?>(url, cancellationToken);
        return produto != null ? Ok(produto) : NotFound();
    }

    [HttpPost("{idCliente}/produto/{codigoProduto}")]
    public async Task<ActionResult<ImpostoProduto>> ImpostoProduto(
        [FromRoute] string idCliente,
        [FromRoute] string codigoProduto,
        [FromBody] ConsultaImpostoProduto consulta,
        CancellationToken cancellationToken)
    {
        var queryModel = new ConsultaImpostoProdutoInterno
        {
            IdCliente = idCliente,
            UsuarioLogado = UsuarioLogado,
            IdProduto = codigoProduto,
            PrecoBase = consulta.PrecoBase,
            PrecoVenda = consulta.PrecoVenda,
            Quantidade = consulta.Quantidade,
            PlanoPagamento = consulta.PlanoPagamento,
            TipoPedido = consulta.TipoPedido,
            Comissao = consulta.Comissao,
            IdPedido = consulta.IdPedido,
            IdRepresentante = consulta.IdRepresentante ?? UsuarioLogado,
            IdGerente = consulta.IdGerente,
            ModoFrete = consulta.ModoFrete
        };

        var url = ConstruirUrlImpostoProduto(queryModel);
        var impostos = await ObterAsync<ImpostoProduto>(url, cancellationToken);
        return Ok(impostos);
    }

    [HttpPost("impostos/lote")]
    public async Task<ActionResult<RespostaImpostoLote>> ObterImpostosLote(
        [FromBody] ConsultaImpostoLote model,
        CancellationToken cancellationToken)
    {
        // Contrato do Protheus definido em protheus/WSTaxes.prw (WSRESTFUL ItensImpostos).
        // Campos aceitos no body: idCliente, idRepresentante, tipoPedido, planoPagamento,
        // itens[].{idProduto, quantidade, precoVenda, precoBase, comissao}.
        // idGerente, modoFrete, idPedido e usuarioLogado são tolerados mas ignorados pelo Protheus.
        var protheusPayload = new ConsultaImpostoLoteProtheus
        {
            IdCliente = model.IdCliente,
            UsuarioLogado = UsuarioLogado,
            TipoPedido = model.TipoPedido,
            PlanoPagamento = model.PlanoPagamento,
            IdRepresentante = model.IdRepresentante ?? UsuarioLogado,
            IdGerente = model.IdGerente,
            ModoFrete = model.ModoFrete,
            IdPedido = model.IdPedido,
            Itens = model.Itens
        };

        var url = $"{CaminhoApiPadrao}/itens/impostos";
        var resultado = await PublicarAsync<RespostaImpostoLote>(protheusPayload, url, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{cnpj}")]
    public async Task<ActionResult<ClienteCompletoDto>> ObterClientePorCnpj([FromRoute] string cnpj, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlClientePorCnpj(UsuarioLogado, cnpj);
        var cliente = await ObterAsync<ClienteCompletoDto?>(url, cancellationToken);
        return cliente != null ? Ok(cliente) : NotFound();
    }

    [HttpGet("{cnpj}/pedidos")]
    public async Task<RespostaPaginada<PedidoListaItem>> ObterPedidosCliente([FromRoute] string cnpj, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlPedidosCliente(UsuarioLogado, cnpj);
        return await ObterAsync<RespostaPaginada<PedidoListaItem>>(url, cancellationToken);
    }

    [HttpGet("dashboard")]
    public async Task<DashboardClientes> ObterDashboardClientes([FromQuery] FiltroPadraoQuery queryObject, CancellationToken cancellationToken)
    {
        var url = ConstruirUrlDashboardClientes(UsuarioLogado, queryObject);
        return await ObterAsync<DashboardClientes>(url, cancellationToken);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ClienteCompletoDto> CriarNovoCliente([FromForm] CriarClienteDto dto, CancellationToken cancellationToken)
    {
        var comprovantes = new List<ArquivoInfo>();
        if (dto.Comprovantes != null)
        {
            var arquivos = dto.Comprovantes.Select(ObterArquivoBase64).ToList();
            await Task.WhenAll(arquivos);

            comprovantes.AddRange(arquivos.Select(x => x.Result!));
        }

        var notasComerciais = new List<ArquivoInfo>();
        if (dto.NotasComerciais != null)
        {
            var arquivos = dto.NotasComerciais.Select(ObterArquivoBase64).ToList();
            await Task.WhenAll(arquivos);

            notasComerciais.AddRange(arquivos.Select(x => x.Result!));
        }

        var novoDto = new ClienteCompletoDto
        {
            UsuarioLogado = UsuarioLogado,
            RazaoSocial = dto.RazaoSocial,
            NomeFantasia = dto.NomeFantasia,
            Endereco = ConverterObjeto<Endereco>(dto.Endereco),
            TelefoneCobranca = dto.TelefoneCobranca,
            Cnpj = dto.Cnpj,
            InscricaoEstadual = dto.InscricaoEstadual,
            Suframa = dto.Suframa,
            DataFundacao = dto.DataFundacao,
            CNAE = dto.CNAE,
            RamoAtividade = dto.RamoAtividade,
            Telefone = dto.Telefone,
            Fax = dto.Fax,
            Celular = dto.Celular,
            NomeContato = dto.NomeContato,
            Email = dto.Email,
            Website = dto.Website,
            CapitalSocial = dto.CapitalSocial,
            Socios = ConverterObjeto<List<SocioInfo>>(dto.Socios),
            EnderecoEntrega = ConverterObjeto<Endereco>(dto.EnderecoEntrega),
            TelefoneEntrega = dto.TelefoneEntrega,
            EmailEntrega = dto.EmailEntrega,
            NomeContatoEntrega = dto.NomeContatoEntrega,
            ReferenciasComerciais = ConverterObjeto<List<ReferenciaComercial>>(dto.ReferenciasComerciais),
            ContratoSocial = await ObterArquivoBase64(dto.ContratoSocial),
            DocumentoSintegra = await ObterArquivoBase64(dto.DocumentoSintegra),
            Comprovantes = comprovantes,
            NotasComerciais = notasComerciais,
            IdRepresentante = dto.IdRepresentante,
            IdGerente = dto.IdGerente,
            DadosBancarios = ConverterObjeto<DadosBancarios>(dto.DadosBancarios)
        };

        var url = ConstruirUrlCriarCliente(UsuarioLogado);
        return await PublicarAsync<ClienteCompletoDto>(novoDto, url, cancellationToken);
    }

    private static T? ConverterObjeto<T>(string? json)
    {
        return string.IsNullOrWhiteSpace(json)
            ? default
            : JsonSerializer.Deserialize<T>(json, JsonOpcoes.Padrao);
    }

    private static async Task<ArquivoInfo?> ObterArquivoBase64(IFormFile? arquivo)
    {
        if (arquivo == null)
        {
            return null;
        }

        using var memoria = new MemoryStream();
        await arquivo.CopyToAsync(memoria);

        memoria.Position = 0;
        var conteudo = memoria.ToArray();

        return new ArquivoInfo
        {
            Nome = arquivo.FileName,
            ConteudoBase64 = Convert.ToBase64String(conteudo)
        };
    }

    private string ConstruirUrlClientesParaPedido(string usuarioLogado, FiltroPadraoQuery queryObject)
        => string.Join('/', CaminhoApiPadrao, "Cliente") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private string ConstruirUrlProdutos(string idCliente, FiltroPadraoQuery queryObject)
    {
        var dict = ObterDicionarioQueryString(null, queryObject);
        dict.Add("IdCliente", idCliente);
        return string.Join('/', CaminhoApiPadrao, "Produto") + ConstruirQueryString(dict);
    }

    private string ConstruirUrlProduto(string idCliente, string codigoProduto)
    {
        var dict = new NameValueCollection
        {
            { "IdCliente", idCliente }
        };
        return string.Join('/', CaminhoApiPadrao, "Produto", codigoProduto) + ConstruirQueryString(dict);
    }

    private string ConstruirUrlImpostoProduto(ConsultaImpostoProdutoInterno model)
    {
        var dict = new NameValueCollection
        {
            { "CodigoProduto", model.IdProduto },
            { "PrecoBase", model.PrecoBase.ToString() },
            { "PrecoVenda", model.PrecoVenda.ToString() },
            { "Quantidade", model.Quantidade.ToString() },
            { "IdCliente", model.IdCliente },
            { "UsuarioLogado", model.UsuarioLogado },
            { "TipoPedido", model.TipoPedido.ToString() },
            { "PlanoPagamento", model.PlanoPagamento },
            { "Comissao", model.Comissao?.ToString() },
            { "IdPedido", model.IdPedido },
            { "IdRepresentante", model.IdRepresentante },
            { "IdGerente", model.IdGerente },
            { "ModoFrete", model.ModoFrete?.ToString() }
        };

        return $"{CaminhoApiPadrao}/Produto" + ConstruirQueryString(dict);
    }

    private string ConstruirUrlClientePorCnpj(string usuarioLogado, string cnpj)
        => string.Join('/', CaminhoApiPadrao, "Cliente", cnpj) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, null));

    private string ConstruirUrlPedidosCliente(string usuarioLogado, string cnpj)
        => string.Join('/', CaminhoApiPadrao, "Cliente", cnpj, "pedidos") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, null));

    private string ConstruirUrlDashboardClientes(string usuarioLogado, FiltroPadraoQuery queryObject)
        => ObterCaminhoDashboard(string.Join('/', CaminhoApiPadrao, "Cliente")) + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, queryObject));

    private string ConstruirUrlCriarCliente(string usuarioLogado)
        => string.Join('/', CaminhoApiPadrao, "Cliente") + ConstruirQueryString(ObterDicionarioQueryString(usuarioLogado, null));
}
