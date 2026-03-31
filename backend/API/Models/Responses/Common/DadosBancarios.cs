namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record DadosBancarios
{
    public required string NomeBanco { get; init; }
    public required string NumeroBanco { get; init; }
    public required string NomeAgencia { get; init; }
    public required string NumeroAgencia { get; init; }
    public required string NumeroConta { get; init; }
}
