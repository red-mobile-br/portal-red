using System.Text.Json;

namespace RedMobilePedidos.API.Settings;

internal static class JsonOpcoes
{
    internal static readonly JsonSerializerOptions Padrao = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}