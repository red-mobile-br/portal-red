namespace RedMobilePedidos.API.Models.Email;

/// <summary>
/// Model para o template de e-mail de pedido.
/// </summary>
public sealed record EmailPedidoModel
{
    public required string IdPedido { get; init; }
    public required string RazaoSocialCliente { get; init; }
}
