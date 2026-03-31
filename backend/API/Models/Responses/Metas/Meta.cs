using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.API.Models.Responses.Metas;

public sealed record Meta
{
    public required string Titulo { get; init; }
    public required DateTime DataInicio { get; init; }
    public required DateTime DataFim { get; init; }
    public required decimal ValorMeta { get; init; }
    public required decimal ValorRealizado { get; init; }
    public required StatusMeta Status { get; init; }
    public required string Representante { get; init; }
    public required string NomeRepresentante { get; init; }
}
