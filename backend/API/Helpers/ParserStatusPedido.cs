using System.Collections.Frozen;

namespace RedMobilePedidos.API.Helpers;

/// <summary>
/// Parser para códigos de status de pedido, compatível com a implementação do frontend.
/// Mapeia códigos de status para texto de exibição e cor.
/// </summary>
public static class ParserStatusPedido
{
    public sealed record InfoStatusPedido(string Titulo, string Cor);

    private static readonly FrozenDictionary<string, InfoStatusPedido> MapaStatus = new Dictionary<string, InfoStatusPedido>
    {
        ["A"] = new("Todos em aberto", "#98D8D5"),
        ["0"] = new("Novo pedido", "#98D8D5"),
        ["1"] = new("Em análise comercial", "#D689CE"),
        ["2"] = new("Em análise financeira", "#BED051"),
        ["3"] = new("Em separação", "#4D93BA"),
        ["4"] = new("Aguardando faturamento", "#4DBA78"),
        ["F"] = new("Faturado", "#7AC185"),
        ["P"] = new("Faturado parcialmente", "#BED051"),
        ["X"] = new("Cancelado", "#F66A70"),
        ["N"] = new("Negado financeiro", "#F66A70"),
        ["M"] = new("Bloqueado por margem", "#BED051"),
        ["R"] = new("Rejeitado por margem", "#F66A70"),
        ["V"] = new("Revisão financeiro", "#4DBA78")
    }.ToFrozenDictionary();

    private static readonly InfoStatusPedido StatusPadrao = new("Desconhecido", "#9E9E9E");

    /// <summary>
    /// Retorna as informações de status para o código informado.
    /// </summary>
    public static InfoStatusPedido ObterStatus(string? codigoStatus)
        => string.IsNullOrEmpty(codigoStatus)
            ? StatusPadrao
            : MapaStatus.GetValueOrDefault(codigoStatus, StatusPadrao);

    /// <summary>
    /// Retorna o título de exibição para o código de status informado.
    /// </summary>
    public static string ObterTituloStatus(string? codigoStatus)
        => ObterStatus(codigoStatus).Titulo;

    /// <summary>
    /// Retorna a cor para o código de status informado.
    /// </summary>
    public static string ObterCorStatus(string? codigoStatus)
        => ObterStatus(codigoStatus).Cor;
}
