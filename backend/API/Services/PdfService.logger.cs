namespace RedMobilePedidos.API.Services;

internal partial class PdfService
{
    [LoggerMessage(LogLevel.Information, "PDF gerado com sucesso, tamanho: {tamanho} bytes")]
    static partial void LogPdfGeradoComSucesso(ILogger<PdfService> logger, int tamanho);

    [LoggerMessage(LogLevel.Error, "Operação do navegador expirou")]
    static partial void LogOperacaoNavegadorExpirou(ILogger<PdfService> logger, Exception ex);

    [LoggerMessage(LogLevel.Error, "Processo do navegador falhou ao iniciar")]
    static partial void LogProcessoNavegadorFalhouAoIniciar(ILogger<PdfService> logger, Exception ex);

    [LoggerMessage(LogLevel.Error, "Falha ao gerar PDF a partir do HTML")]
    static partial void LogFalhaAoGerarPdfDeHtml(ILogger<PdfService> logger, Exception ex);
}
