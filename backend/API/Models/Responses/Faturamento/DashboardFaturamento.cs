using RedMobilePedidos.API.Models.Responses.Common;

namespace RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;

public sealed record DashboardFaturamento
{
    public required decimal TotalFaturado { get; init; }
    public required decimal TotalBase { get; init; }
    public required int QuantidadeVendas { get; init; }
    public required List<ItemGrafico> VendasPorCategoria { get; init; }
    public required List<ProdutoMaisVendido> ProdutosMaisVendidos { get; init; }
    public required List<ItemGrafico> VendasPorDia { get; init; }
}

public sealed record ProdutoPorCategoria
{
    public required string Categoria { get; init; }
    public required int Vendas { get; init; }
}

public sealed record ProdutoMaisVendido
{
    public required string UrlImagem { get; init; }
    public required string Nome { get; init; }
    public required int Unidades { get; init; }
}

public sealed record VendasPorDia
{
    public required string Dia { get; init; }
    public required decimal ValorFatura { get; init; }
    public required decimal ValorBase { get; init; }
}
