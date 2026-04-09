using System.Text.Json;
using System.Text.Json.Serialization;
using RedMobilePedidos.API.Settings.Converters;

namespace RedMobilePedidos.API.Settings;

internal static class JsonOpcoes
{
    internal static readonly JsonSerializerOptions Padrao = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new ConversorDataProtheus() }
    };
}
