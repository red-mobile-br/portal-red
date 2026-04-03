using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public record PedidoListaItem
{
    public string? Id { get; init; }
    public string? Status { get; init; }
    public DateTime? DataLancamento { get; init; }
    public string? Cnpj { get; init; }
    public string? IdCliente { get; init; }
    public string? Nome { get; init; }
    public decimal? ValorTotal { get; init; }
    public List<Nota>? Notas { get; init; }
    public List<NotaFiscal>? NotasFiscais { get; init; }
    public List<string>? Boletos { get; init; }
    public string? Representante { get; init; }
    public string? NomeRepresentante { get; init; }
}
