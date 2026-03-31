namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record Nota
{
    public required string Atividade { get; init; }
    public required string Detalhe { get; init; }
    public required DateTime DataHora { get; init; }
    public required string Autor { get; init; }
}
