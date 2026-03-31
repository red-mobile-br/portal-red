namespace RedMobilePedidos.API.Models.Responses.Common;

public record RespostaPadrao
{
    public bool Sucesso { get; init; }

    public string? Detalhes { get; init; }
}