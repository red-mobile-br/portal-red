namespace RedMobilePedidos.API.Models.Requests;

public sealed record ItemImpostoLote
{
    public required string IdProduto { get; init; }
    public decimal? PrecoBase { get; init; }
    public decimal? PrecoVenda { get; init; }
    public int? Quantidade { get; init; }
    public decimal? Comissao { get; init; }
}

/// <summary>
/// Payload recebido do frontend para consulta de impostos em lote.
/// </summary>
public sealed record ConsultaImpostoLote
{
    public required string IdCliente { get; init; }
    public string? IdRepresentante { get; init; }
    public string? IdGerente { get; init; }
    public int? TipoPedido { get; init; }
    public string? PlanoPagamento { get; init; }
    public int? ModoFrete { get; init; }
    public string? IdPedido { get; init; }
    public required List<ItemImpostoLote> Itens { get; init; }
}

/// <summary>
/// Payload enviado ao Protheus POST /itens/impostos (WSTaxes.prw → WSRESTFUL ItensImpostos).
/// Segue o contrato definido em protheus/WSTaxes.prw.
/// </summary>
public sealed record ConsultaImpostoLoteProtheus
{
    public required string IdCliente { get; init; }
    public string? UsuarioLogado { get; init; }
    public int? TipoPedido { get; init; }
    public string? PlanoPagamento { get; init; }
    public string? IdRepresentante { get; init; }
    public string? IdGerente { get; init; }
    public int? ModoFrete { get; init; }
    public string? IdPedido { get; init; }
    public required List<ItemImpostoLote> Itens { get; init; }
}
