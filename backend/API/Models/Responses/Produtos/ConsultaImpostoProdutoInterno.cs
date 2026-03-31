namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record ConsultaImpostoProdutoInterno
{
    public required string IdProduto { get; init; }
    public decimal? PrecoBase { get; init; }
    public decimal? PrecoVenda { get; init; }
    public int? Quantidade { get; init; }
    public string? IdCliente { get; init; }
    public string? UsuarioLogado { get; init; }
    public int? TipoPedido { get; init; }
    public string? PlanoPagamento { get; init; }
    public decimal? Comissao { get; init; }
    public string? IdPedido { get; init; }
    public string? IdRepresentante { get; init; }
    public string? IdGerente { get; init; }
    public int? ModoFrete { get; init; }
}
