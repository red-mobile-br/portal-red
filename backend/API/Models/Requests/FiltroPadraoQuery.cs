namespace RedMobilePedidos.API.Models.Requests;

public sealed record FiltroPadraoQuery
{
    public DateTime? De { get; init; }
    public DateTime? Ate { get; init; }
    public int? Pagina { get; init; }
    public int? Tamanho { get; init; }
    public string? OrdenarPor { get; init; }
    public string? Direcao { get; init; }
    public string? Busca { get; init; }
    public string? Status { get; init; }
    public string? Cnpj { get; init; }
    public string? FiltroUsuario { get; init; }
    public string? IdGerente { get; init; }
    public string? IdRepresentante { get; init; }
    public string? IdCliente { get; init; }
}
