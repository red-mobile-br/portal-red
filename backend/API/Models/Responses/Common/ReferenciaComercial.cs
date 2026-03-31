namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record ReferenciaComercial
{
    public required string RazaoSocial { get; init; }
    public required string Telefone { get; init; }
    public required string Celular { get; init; }
    public required string NomeContato { get; init; }
}
