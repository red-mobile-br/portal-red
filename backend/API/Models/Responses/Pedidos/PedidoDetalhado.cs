using RedMobilePedidos.API.Enums;
using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public record PedidoDetalhado
{
    public required string Id { get; init; }
    public required ClientePedido Cliente { get; init; }
    public required ModoFrete ModoFrete { get; init; }
    public DadosAgendamento? DadosAgendamento { get; init; }
    public required Endereco EnderecoEntrega { get; init; }
    public required List<ProdutoPedido> Produtos { get; init; }
    public required string NumeroPedidoCompra { get; init; }
    public required DateTime DataEmissao { get; init; }
    public required string InformacoesNota { get; init; }
    public required string ObservacoesPedido { get; init; }
    public required List<Nota> Notas { get; init; }
    public required DateTime DataLancamento { get; init; }
    public required decimal ValorProdutos { get; init; }
    public required decimal ValorICMS { get; init; }
    public required decimal ValorICMSST { get; init; }
    public required decimal ValorIPI { get; init; }
    public required decimal ValorNota { get; init; }
    public required List<NotaFiscal> NotasFiscais { get; init; }
    public required List<string> Boletos { get; init; }
    public required string Status { get; init; }
    public required string PlanoPagamento { get; init; }
    public required TipoPedido TipoPedido { get; init; }
    public required string IdRepresentante { get; init; }
    public required string NomeRepresentante { get; init; }
    public required decimal MargemPedido { get; init; }
    public required string IdGerente { get; init; }
    public required string NomeGerente { get; init; }
    public required decimal PesoLiquido { get; init; }
    public required decimal PesoBruto { get; init; }
}

public sealed record ClientePedido
{
    public required string Id { get; init; }
    public required string NomeFantasia { get; init; }
    public required string RazaoSocial { get; init; }
    public required string Cnpj { get; init; }
    public required Endereco Endereco { get; init; }
}

public sealed record ProdutoPedido
{
    public required int Ordem { get; init; }
    public required string Id { get; init; }
    public required string Descricao { get; init; }
    public required int Quantidade { get; init; }
    public required decimal PercentualDesconto { get; init; }
    public required decimal ValorUnitario { get; init; }
    public required decimal PrecoBase { get; init; }
    public required decimal Comissao { get; init; }
    public required decimal ValorTotal { get; init; }
    public required decimal Margem { get; init; }
    public required decimal PercentualICMS { get; init; }
    public required decimal PercentualIPI { get; init; }
    public required decimal PercentualICMSST { get; init; }
    public required int SaldoPendente { get; init; }
    public required decimal ICMS { get; init; }
    public required decimal ICMSST { get; init; }
    public required decimal IPI { get; init; }
    public required decimal ComissaoMaxima { get; init; }
}
