using System.ComponentModel.DataAnnotations;

namespace RedMobilePedidos.API.Models.Requests;

public sealed record LoginModel
{
    [Required]
    public required string NomeUsuario { get; init; }

    [Required]
    public required string Senha { get; init; }
}
