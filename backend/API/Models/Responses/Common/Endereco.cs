namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record Endereco
{
    public string? Logradouro { get; init; }
    public int? Numero { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string? Cidade { get; init; }
    public string? Uf { get; init; }
    public string? Cep { get; init; }
}
