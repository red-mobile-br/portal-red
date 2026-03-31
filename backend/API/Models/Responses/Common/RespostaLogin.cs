namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record RespostaLogin
{
    public required bool Sucesso { get; init; }

    public required bool Gerente { get; init; }

    public required string Detalhes { get; init; }
}