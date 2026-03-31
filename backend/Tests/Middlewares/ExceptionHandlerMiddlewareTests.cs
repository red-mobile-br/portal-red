using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using RedMobilePedidos.API.Exceptions;
using RedMobilePedidos.API.Middlewares;
using RedMobilePedidos.API.Models.Responses.Common;
using Xunit;

namespace RedMobilePedidos.Tests.Middlewares;

public class ExceptionHandlerMiddlewareTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Invoke_ShouldPassThrough_WhenNoExceptionOccurs()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        var proximoChamado = false;

        var middleware = new ExceptionHandlerMiddleware(_ =>
        {
            proximoChamado = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.Invoke(contexto);

        // Assert
        proximoChamado.Should().BeTrue();
        contexto.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Invoke_ShouldReturnModeloErro_WhenFalhaNaRequisicaoExceptionOccurs()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();

        var erroProtheus = new ModeloErro
        {
            Codigo = 400,
            Mensagem = "Dados do cliente inválidos",
            MensagemDetalhada = "CNPJ já existe"
        };
        var conteudoErro = JsonSerializer.Serialize(erroProtheus, _jsonOpcoes);

        var middleware = new ExceptionHandlerMiddleware(_ =>
            throw new FalhaNaRequisicaoException(conteudoErro));

        // Act
        await middleware.Invoke(contexto);

        // Assert
        contexto.Response.StatusCode.Should().Be(400);
        contexto.Response.ContentType.Should().Be("application/json");

        contexto.Response.Body.Position = 0;
        using var leitor = new StreamReader(contexto.Response.Body);
        var corpoResposta = await leitor.ReadToEndAsync();
        var resultado = JsonSerializer.Deserialize<ModeloErro>(corpoResposta, _jsonOpcoes);

        resultado.Should().NotBeNull();
        resultado!.Codigo.Should().Be(400);
        resultado.Mensagem.Should().Be("Dados do cliente inválidos");
        resultado.MensagemDetalhada.Should().Be("CNPJ já existe");
    }

    [Fact]
    public async Task Invoke_ShouldReturnFallbackError_WhenFalhaNaRequisicaoExceptionHasInvalidJson()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();

        var jsonInvalido = "Isso não é um JSON válido";

        var middleware = new ExceptionHandlerMiddleware(_ =>
            throw new FalhaNaRequisicaoException(jsonInvalido));

        // Act
        await middleware.Invoke(contexto);

        // Assert
        contexto.Response.StatusCode.Should().Be(400);

        contexto.Response.Body.Position = 0;
        using var leitor = new StreamReader(contexto.Response.Body);
        var corpoResposta = await leitor.ReadToEndAsync();
        var resultado = JsonSerializer.Deserialize<ModeloErro>(corpoResposta, _jsonOpcoes);

        resultado.Should().NotBeNull();
        resultado!.Codigo.Should().Be(400);
        resultado.Mensagem.Should().Be(jsonInvalido);
    }

    [Fact]
    public async Task Invoke_ShouldReturnGenericError_WhenGenericExceptionOccurs()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(_ =>
            throw new Exception("Algo deu errado"));

        // Act
        await middleware.Invoke(contexto);

        // Assert
        contexto.Response.StatusCode.Should().Be(400);
        contexto.Response.ContentType.Should().Be("application/json");

        contexto.Response.Body.Position = 0;
        using var leitor = new StreamReader(contexto.Response.Body);
        var corpoResposta = await leitor.ReadToEndAsync();
        var resultado = JsonSerializer.Deserialize<ModeloErro>(corpoResposta, _jsonOpcoes);

        resultado.Should().NotBeNull();
        resultado!.Codigo.Should().Be(400);
        resultado.Mensagem.Should().Be("Algo deu errado");
    }

    [Fact]
    public async Task Invoke_ShouldReturnProtheusConnectionError_WhenSocketExceptionOccurs()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();

        var excecaoSocket = new SocketException();
        var excecaoHttp = new HttpRequestException("Conexão recusada", excecaoSocket);

        var middleware = new ExceptionHandlerMiddleware(_ =>
            throw excecaoHttp);

        // Act
        await middleware.Invoke(contexto);

        // Assert
        contexto.Response.StatusCode.Should().Be(400);

        contexto.Response.Body.Position = 0;
        using var leitor = new StreamReader(contexto.Response.Body);
        var corpoResposta = await leitor.ReadToEndAsync();
        var resultado = JsonSerializer.Deserialize<ModeloErro>(corpoResposta, _jsonOpcoes);

        resultado.Should().NotBeNull();
        resultado!.Codigo.Should().Be(400);
        resultado.Mensagem.Should().Be("Não foi possível se comunicar com o servidor Protheus.");
    }
}
