namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record ArquivoInfo
{
    public required string Nome { get; init; }
    public required string ConteudoBase64 { get; init; }
}
