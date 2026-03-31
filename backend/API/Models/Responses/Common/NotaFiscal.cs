namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record NotaFiscal
{
    public required string Codigo { get; init; }
    public string? UrlRastreio { get; init; }
}
