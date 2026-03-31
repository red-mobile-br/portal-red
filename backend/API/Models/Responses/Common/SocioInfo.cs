namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record SocioInfo
{
    public required string Nome { get; init; }
    public required string Percentual { get; init; }
    public required string CPF { get; init; }
}
