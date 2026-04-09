using System.Text.Json;
using System.Text.Json.Serialization;

namespace RedMobilePedidos.API.Settings.Converters;

/// <summary>
/// Converte strings ISO do Protheus em DateTime?.
/// Trata como null: tokens Null, strings vazias/whitespace, datas malformadas
/// (ex: "----T00:00:00.0000" que o WSPriceT.prw gera quando DA0_DATATE é vazio).
/// </summary>
public sealed class ConversorDataProtheus : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var texto = reader.GetString();
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            return DateTime.TryParse(texto, out var data) ? data : null;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value);
    }
}
