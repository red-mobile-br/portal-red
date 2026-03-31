using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.API.Models.Responses.Common;

public sealed record Usuario
{
    public required string IdRepresentante { get; init; }
    public required string NomeUsuario { get; init; }
    public required string Nome { get; init; }
    public required string UrlFoto { get; init; }
    public required TipoUsuario TipoUsuario { get; init; }
}
