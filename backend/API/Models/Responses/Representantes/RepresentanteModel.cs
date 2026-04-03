namespace RedMobilePedidos.API.Models.Responses.Representantes;

public sealed record RepresentanteModel
{
    public string? Id { get; init; }
    public string? Nome { get; init; }
    public string? Cnpj { get; init; }
}
