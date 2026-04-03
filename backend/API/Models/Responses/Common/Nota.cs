namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record Nota
{
    public string? Atividade { get; init; }
    public string? Detalhe { get; init; }
    public DateTime? DataHora { get; init; }
    public string? Autor { get; init; }
}
