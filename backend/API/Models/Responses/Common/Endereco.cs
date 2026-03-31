namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record Endereco
{
    public string? Logradouro { get; init; }
    public string? Numero { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public string? Cep { get; init; }
}
