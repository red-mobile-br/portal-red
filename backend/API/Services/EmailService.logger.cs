using MailKit.Net.Smtp;

namespace RedMobilePedidos.API.Services;

internal partial class EmailService
{
    [LoggerMessage(LogLevel.Information, "E-mail enviado com sucesso para {emailDestinatario}")]
    static partial void LogEmailEnviadoComSucesso(ILogger<EmailService> logger, string emailDestinatario);

    [LoggerMessage(LogLevel.Information, "Enviando e-mail para {emailDestinatario} com assunto: {assunto}")]
    static partial void LogEnviandoEmailComAssunto(ILogger<EmailService> logger, string emailDestinatario, string assunto);

    [LoggerMessage(LogLevel.Error, "Erro de comando SMTP ao enviar e-mail para {emailDestinatario}: {codigoErro}")]
    static partial void LogErroComandoSmtpAoEnviarEmail(ILogger<EmailService> logger, string emailDestinatario, SmtpErrorCode codigoErro, Exception ex);

    [LoggerMessage(LogLevel.Error, "Erro de protocolo SMTP ao enviar e-mail para {emailDestinatario}")]
    static partial void LogErroProtocoloSmtpAoEnviarEmail(ILogger<EmailService> logger, string emailDestinatario, Exception ex);

    [LoggerMessage(LogLevel.Error, "Autenticação SMTP falhou")]
    static partial void LogAutenticacaoSmtpFalhou(ILogger<EmailService> logger, Exception ex);

    [LoggerMessage(LogLevel.Error, "Falha ao enviar e-mail para {emailDestinatario}")]
    static partial void LogFalhaAoEnviarEmail(ILogger<EmailService> logger, string emailDestinatario, Exception ex);
}
