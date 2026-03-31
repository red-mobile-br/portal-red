namespace RedMobilePedidos.API.Models.Settings;

public sealed record SmtpSettings
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required bool EnableSsl { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public required string FromName { get; init; }
    public required string FromEmail { get; init; }
}
