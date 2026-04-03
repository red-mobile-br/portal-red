namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record ArquivoInfo
{
    public string? Nome { get; init; }
    public string? ConteudoBase64 { get; init; }
}
