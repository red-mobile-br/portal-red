using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Requests;

public sealed record TokenModel
{
    public required string TokenAcesso { get; init; }

    public required DateTime ExpiraEm { get; init; }
    public required Usuario DadosUsuario { get; init; }
}