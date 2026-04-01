namespace RedMobilePedidos.API.Settings;

public sealed record ProtheusSettings
{
    public required string BaseUrl { get; init; }
    public required string Usuario { get; init; }
    public required string Senha { get; init; }
}
