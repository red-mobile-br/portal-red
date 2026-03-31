namespace RedMobilePedidos.API.Models.Requests;

public sealed record EmailModel
{
    public required string Destinatario { get; init; }
}
