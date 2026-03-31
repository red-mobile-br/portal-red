namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record DadosAgendamento
{
    public required string Email { get; init; }
    public required string Telefone { get; init; }
}
