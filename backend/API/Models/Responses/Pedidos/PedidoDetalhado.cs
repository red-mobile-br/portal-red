using RedMobilePedidos.API.Enums;
using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.Pedidos;

public record PedidoDetalhado
{
    public string? Id { get; init; }
    public ClientePedido? Cliente { get; init; }
    public ModoFrete? ModoFrete { get; init; }
    public DadosAgendamento? DadosAgendamento { get; init; }
    public Endereco? EnderecoEntrega { get; init; }
    public List<ProdutoPedido>? Produtos { get; init; }
    public string? NumeroPedidoCompra { get; init; }
    public DateTime? DataEmissao { get; init; }
    public string? InformacoesNota { get; init; }
    public string? ObservacoesPedido { get; init; }
    public List<Nota>? Anotacoes { get; init; }
    public decimal? ValorProdutos { get; init; }
    public decimal? ValorICMS { get; init; }
    public decimal? ValorICMSST { get; init; }
    public decimal? ValorIPI { get; init; }
    public decimal? ValorNota { get; init; }
    public List<NotaFiscal>? NotasFiscais { get; init; }
    public List<string>? Boletos { get; init; }
    public string? Status { get; init; }
    public string? PlanoPagamento { get; init; }
    public TipoPedido? TipoPedido { get; init; }
    public string? IdRepresentante { get; init; }
    public string? NomeRepresentante { get; init; }
    public decimal? MargemPedido { get; init; }
    public string? IdGerente { get; init; }
    public string? NomeGerente { get; init; }
    public decimal? PesoLiquido { get; init; }
    public decimal? PesoBruto { get; init; }
    public int? TotalItens { get; init; }
    public int? TotalVolumes { get; init; }
}

public sealed record ClientePedido
{
    public string? Id { get; init; }
    public string? NomeFantasia { get; init; }
    public string? RazaoSocial { get; init; }
    public string? Cnpj { get; init; }
    public Endereco? Endereco { get; init; }
}

public sealed record ProdutoPedido
{
    public int? Ordem { get; init; }
    public string? Id { get; init; }
    public string? Descricao { get; init; }
    public int? Quantidade { get; init; }
    public decimal? PercentualDesconto { get; init; }
    public decimal? ValorUnitario { get; init; }
    public decimal? PrecoBase { get; init; }
    public decimal? Comissao { get; init; }
    public decimal? ValorTotal { get; init; }
    public decimal? Margem { get; init; }
    public decimal? PercentualICMS { get; init; }
    public decimal? PercentualIPI { get; init; }
    public decimal? PercentualICMSST { get; init; }
    public int? SaldoPendente { get; init; }
    public decimal? ComissaoMaxima { get; init; }
}
