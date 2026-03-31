using RedMobilePedidos.API.Enums;
using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Requests;

public sealed record CriarPedido
{
    public required string IdCliente { get; init; }
    public required ModoFrete ModoFrete { get; init; }
    public DadosAgendamento? DadosAgendamento { get; init; }
    public required Endereco EnderecoEntrega { get; init; }
    public required List<CriarPedidoProduto> Produtos { get; init; }
    public string? NumeroPedidoCompra { get; init; }
    public required DateTime DataEmissao { get; init; }
    public string? InformacoesNota { get; init; }
    public string? ObservacoesPedido { get; init; }
    public required TipoPedido TipoPedido { get; init; }
    public string? PlanoPagamento { get; init; }
    public string? IdRepresentante { get; init; }
    public string? IdGerente { get; init; }
}

public sealed record CriarPedidoProduto
{
    public required string Id { get; init; }
    public required int Quantidade { get; init; }
    public required decimal ValorUnitario { get; init; }
    public required decimal Comissao { get; init; }
}
