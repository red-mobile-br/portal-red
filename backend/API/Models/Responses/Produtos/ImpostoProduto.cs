namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record ImpostoProduto
{
    public decimal? ICMS { get; init; }
    public decimal? ICMSST { get; init; }
    public decimal? IPI { get; init; }
    public decimal? Comissao { get; init; }
    public decimal? Margem { get; init; }
    public decimal? PercentualICMS { get; init; }
    public decimal? PercentualIPI { get; init; }
    public decimal? PercentualICMSST { get; init; }
    public decimal? ComissaoMaxima { get; init; }
    public decimal? ValorTotal { get; init; }
}
