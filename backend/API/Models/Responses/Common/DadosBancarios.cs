namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record DadosBancarios
{
    public string? NomeBanco { get; init; }
    public string? NumeroBanco { get; init; }
    public string? NomeAgencia { get; init; }
    public string? NumeroAgencia { get; init; }
    public string? NumeroConta { get; init; }
}
