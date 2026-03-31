namespace RedMobilePedidos.API.Models.Responses.Representantes;

public sealed record RepresentanteModel
{
    public required string Id { get; init; }
    public required string Nome { get; init; }
    public required string Cnpj { get; init; }
}
