using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RedMobilePedidos.API.Models.Settings;

namespace RedMobilePedidos.API.Services;

internal partial class EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task EnviarEmailAsync(string emailDestinatario, string assunto, string corpo, byte[]? anexo = null, string? nomeArquivoAnexo = null)
    {
        LogEnviandoEmailComAssunto(logger, emailDestinatario, assunto);

        try
        {
            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            mensagem.To.Add(MailboxAddress.Parse(emailDestinatario));
            mensagem.Subject = assunto;

            var construtor = new BodyBuilder
            {
                HtmlBody = corpo
            };

            if (anexo != null && !string.IsNullOrEmpty(nomeArquivoAnexo))
            {
                construtor.Attachments.Add(nomeArquivoAnexo, anexo);
            }

            mensagem.Body = construtor.ToMessageBody();

            using var client = new SmtpClient();

            var opcaoSocketSeguro = _smtpSettings.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None;

            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, opcaoSocketSeguro);

            // Autentica somente se credenciais foram fornecidas e o servidor suporta autenticação
            var temCredenciais = !string.IsNullOrEmpty(_smtpSettings.Username) && !string.IsNullOrEmpty(_smtpSettings.Password);
            var servidorSuportaAuth = client.Capabilities.HasFlag(SmtpCapabilities.Authentication);

            if (temCredenciais && servidorSuportaAuth)
            {
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            }

            await client.SendAsync(mensagem);
            await client.DisconnectAsync(true);

            LogEmailEnviadoComSucesso(logger, emailDestinatario);
        }
        catch (SmtpCommandException ex)
        {
            LogErroComandoSmtpAoEnviarEmail(logger, emailDestinatario, ex.ErrorCode, ex);
            throw;
        }
        catch (SmtpProtocolException ex)
        {
            LogErroProtocoloSmtpAoEnviarEmail(logger, emailDestinatario, ex);
            throw;
        }
        catch (AuthenticationException ex)
        {
            LogAutenticacaoSmtpFalhou(logger, ex);
            throw;
        }
        catch (Exception ex)
        {
            LogFalhaAoEnviarEmail(logger, emailDestinatario, ex);
            throw;
        }
    }
}
