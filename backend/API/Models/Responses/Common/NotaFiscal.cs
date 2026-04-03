namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record NotaFiscal
{
    public string? Codigo { get; init; }
    public string? UrlRastreio { get; init; }
}
