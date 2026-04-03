namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record ReferenciaComercial
{
    public string? RazaoSocial { get; init; }
    public string? Telefone { get; init; }
    public string? Celular { get; init; }
    public string? NomeContato { get; init; }
}
