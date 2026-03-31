using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public record PedidoListaItem
{
    public required string Id { get; init; }
    public required string Status { get; init; }
    public required DateTime DataLancamento { get; init; }
    public required string Cnpj { get; init; }
    public required string IdCliente { get; init; }
    public required string Nome { get; init; }
    public required decimal ValorTotal { get; init; }
    public required List<Nota> Notas { get; init; }
    public required List<NotaFiscal> NotasFiscais { get; init; }
    public required List<string> Boletos { get; init; }
    public required string Representante { get; init; }
    public required string NomeRepresentante { get; init; }
}
