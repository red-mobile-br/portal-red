namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record SocioInfo
{
    public string? Nome { get; init; }
    public decimal? Percentual { get; init; }
    public string? Cpf { get; init; }
}
