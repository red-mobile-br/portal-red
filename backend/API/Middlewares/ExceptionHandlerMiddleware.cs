using System.Text.Json;
using RedMobilePedidos.API.Exceptions;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Settings;
using System.Net.Sockets;


namespace RedMobilePedidos.API.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FalhaNaRequisicaoException ex)
        {
            await TratarErroProtheus(context, ex.Content);
        }
        catch (Exception ex)
        {
            await TratarErroGenerico(context, ex);
        }
    }

    /// <summary>
    /// Trata erros retornados pelo Protheus, normalizando o formato da resposta.
    /// </summary>
    private static async Task TratarErroProtheus(HttpContext context, string conteudo)
    {
        var resposta = context.Response;
        resposta.ContentType = "application/json";
        resposta.StatusCode = StatusCodes.Status400BadRequest;

        // Tenta interpretar o erro do Protheus e normalizá-lo
        try
        {
            var erroProtheus = JsonSerializer.Deserialize<ModeloErro>(conteudo, JsonOpcoes.Padrao);
            if (erroProtheus != null)
            {
                var resultado = JsonSerializer.Serialize(erroProtheus, JsonOpcoes.Padrao);
                await resposta.WriteAsync(resultado);
                return;
            }
        }
        catch (JsonException)
        {
            // Se a interpretação falhar, usa o conteúdo bruto como mensagem
        }

        // Fallback: retorna o conteúdo bruto como mensagem de erro
        var modeloErro = new ModeloErro
        {
            Codigo = StatusCodes.Status400BadRequest,
            Mensagem = conteudo
        };
        await resposta.WriteAsync(JsonSerializer.Serialize(modeloErro, JsonOpcoes.Padrao));
    }

    /// <summary>
    /// Trata exceções genéricas com mensagens amigáveis ao usuário.
    /// </summary>
    private static async Task TratarErroGenerico(HttpContext context, Exception ex)
    {
        var resposta = context.Response;
        resposta.ContentType = "application/json";
        resposta.StatusCode = StatusCodes.Status400BadRequest;

        var mensagemErro = ex switch
        {
            // Erros de conexão SMTP
            SocketException => "Não foi possível conectar ao servidor de e-mail. Verifique sua conexão.",
            // Erros de conexão HTTP/Protheus
            HttpRequestException { InnerException: SocketException } => "Não foi possível se comunicar com o servidor Protheus.",
            // Erros de autenticação MailKit
            MailKit.Security.AuthenticationException => "Falha na autenticação do servidor de e-mail.",
            // Erros de comando SMTP MailKit
            MailKit.Net.Smtp.SmtpCommandException smtpEx => $"Erro ao enviar e-mail: {smtpEx.Message}",
            MailKit.Net.Smtp.SmtpProtocolException => "Erro de comunicação com o servidor de e-mail.",
            // Padrão
            _ => ex.Message
        };

        var modeloErro = new ModeloErro
        {
            Codigo = StatusCodes.Status400BadRequest,
            Mensagem = mensagemErro
        };

        var resultado = JsonSerializer.Serialize(modeloErro, JsonOpcoes.Padrao);
        await resposta.WriteAsync(resultado);
    }
}
