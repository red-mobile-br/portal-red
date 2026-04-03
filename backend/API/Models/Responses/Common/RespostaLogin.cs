namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record RespostaLogin
{
    public bool Sucesso { get; init; }
    public bool Gerente { get; init; }
    public string? Detalhes { get; init; }
}
