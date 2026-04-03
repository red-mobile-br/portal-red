namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record SocioInfo
{
    public string? Nome { get; init; }
    public string? Percentual { get; init; }
    public string? CPF { get; init; }
}
