namespace RedMobilePedidos.API.Models.Responses.Produtos;

public sealed record ImpostoProduto
{
    public required decimal ICMS { get; init; }
    public required decimal ICMSST { get; init; }
    public required decimal IPI { get; init; }
    public required decimal Comissao { get; init; }
    public required decimal Margem { get; init; }
    public required decimal PercentualICMS { get; init; }
    public required decimal PercentualIPI { get; init; }
    public required decimal PercentualICMSST { get; init; }
    public required decimal ComissaoMaxima { get; init; }
    public required decimal ValorTotal { get; init; }
}
