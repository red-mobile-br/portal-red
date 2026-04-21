using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;

public sealed record DashboardFaturamento
{
    public decimal? TotalNF { get; init; }
    public decimal? TotalBase { get; init; }
    public int? QuantidadeVendas { get; init; }
    public List<ItemGrafico>? VendasPorCategoria { get; init; }
    public List<ProdutoMaisVendido>? ProdutosMaisVendidos { get; init; }
    public List<ItemGrafico>? VendasPorDia { get; init; }
}

public sealed record ProdutoPorCategoria
{
    public string? Categoria { get; init; }
    public int? Vendas { get; init; }
}

public sealed record ProdutoMaisVendido
{
    public string? UrlImagem { get; init; }
    public string? Nome { get; init; }
    public int? Unidades { get; init; }
}

public sealed record VendasPorDia
{
    public string? Dia { get; init; }
    public decimal? ValorFatura { get; init; }
    public decimal? ValorBase { get; init; }
}
