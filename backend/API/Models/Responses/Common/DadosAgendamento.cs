namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record DadosAgendamento
{
    public string? Email { get; init; }
    public string? Telefone { get; init; }
}
