using RedMobilePedidos.API.Models.Responses.Common;
using ArquivoInfo = RedMobilePedidos.API.Models.Responses.Common.ArquivoInfo;

namespace RedMobilePedidos.API.Models.Responses.Clientes;

public sealed record ClienteCompletoDto
{
    public string? Id { get; init; }
    public string? TipoCliente { get; init; }
    public string? IdRepresentante { get; init; }
    public string? NomeRepresentante { get; init; }
    public string? IdGerente { get; init; }
    public string? NomeGerente { get; init; }
    public string? UsuarioLogado { get; init; }

    public string? RazaoSocial { get; init; }
    public string? NomeFantasia { get; init; }

    public Endereco? Endereco { get; init; }
    public string? TelefoneCobranca { get; init; }
    public string? Cnpj { get; init; }
    public string? InscricaoEstadual { get; init; }
    public string? Suframa { get; init; }
    public string? DataFundacao { get; init; }
    public string? CNAE { get; init; }
    public string? RamoAtividade { get; init; }

    public string? Telefone { get; init; }
    public string? Fax { get; init; }
    public string? Celular { get; init; }
    public string? NomeContato { get; init; }
    public string? Email { get; init; }
    public string? Website { get; init; }

    public double? CapitalSocial { get; init; }
    public List<SocioInfo>? Socios { get; init; }

    public Endereco? EnderecoEntrega { get; init; }
    public string? TelefoneEntrega { get; init; }
    public string? EmailEntrega { get; init; }
    public string? NomeContatoEntrega { get; init; }

    public List<ReferenciaComercial>? ReferenciasComerciais { get; init; }
    public ArquivoInfo? ContratoSocial { get; init; }
    public ArquivoInfo? DocumentoSintegra { get; init; }
    public List<ArquivoInfo>? Comprovantes { get; init; }
    public List<ArquivoInfo>? NotasComerciais { get; init; }
    public DadosBancarios? DadosBancarios { get; init; }
}
