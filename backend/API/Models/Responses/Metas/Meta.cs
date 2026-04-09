using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.API.Models.Responses.Metas;

public sealed record Meta
{
    public string? Titulo { get; init; }
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public decimal? ValorMeta { get; init; }
    public decimal? ValorRealizado { get; init; }
    public StatusMeta? Status { get; init; }
    public string? IdRepresentante { get; init; }
    public string? NomeRepresentante { get; init; }
}
