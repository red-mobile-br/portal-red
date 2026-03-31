namespace RedMobilePedidos.API.Exceptions;

public class FalhaNaRequisicaoException(string content) : Exception(ExtractMessage(content))
{
    public string Content { get; } = content;

    private static string ExtractMessage(string content)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("mensagem", out var mensagem))
                return mensagem.GetString() ?? content;

            if (root.TryGetProperty("mensagemDetalhada", out var mensagemDetalhada))
                return mensagemDetalhada.GetString() ?? content;

            return content;
        }
        catch
        {
            return content;
        }
    }
}