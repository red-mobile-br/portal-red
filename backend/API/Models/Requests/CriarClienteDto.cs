namespace RedMobilePedidos.API.Models.Requests;

public sealed record CriarClienteDto
{
    public string? RazaoSocial { get; init; }
    public string? NomeFantasia { get; init; }

    public string? Endereco { get; init; }
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
    public string? Socios { get; init; }

    public string? EnderecoEntrega { get; init; }
    public string? TelefoneEntrega { get; init; }
    public string? EmailEntrega { get; init; }
    public string? NomeContatoEntrega { get; init; }

    public string? ReferenciasComerciais { get; init; }
    public string? DadosBancarios { get; init; }
    public IFormFile? ContratoSocial { get; init; }
    public IFormFile? DocumentoSintegra { get; init; }
    public IFormFileCollection? Comprovantes { get; init; }
    public IFormFileCollection? NotasComerciais { get; init; }

    public string? IdRepresentante { get; init; }
    public string? IdGerente { get; init; }
}
