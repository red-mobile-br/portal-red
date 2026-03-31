namespace RedMobilePedidos.API.Services;

public interface IEmailService
{
    Task EnviarEmailAsync(string emailDestinatario, string assunto, string corpo, byte[]? anexo = null, string? nomeArquivoAnexo = null);
}
