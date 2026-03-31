namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record ModeloErro
{
    public required int Codigo { get; init; }

    public required string Mensagem { get; init; }

    public string? MensagemDetalhada { get; init; }
}