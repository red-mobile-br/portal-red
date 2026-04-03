using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record Usuario
{
    public string? IdRepresentante { get; init; }
    public string? Nome { get; init; }
    public string? NomeUsuario { get; init; }
    public string? UrlFoto { get; init; }
    public TipoUsuario? TipoUsuario { get; init; }
}
