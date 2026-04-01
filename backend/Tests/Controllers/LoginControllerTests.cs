using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using RedMobilePedidos.API.Controllers;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Models.Responses.Common;
using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;
using RedMobilePedidos.Tests.Builders;
using RedMobilePedidos.Tests.Helpers;
using Xunit;

namespace RedMobilePedidos.Tests.Controllers;

public class LoginControllerTests : BaseControllerTests
{
    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly Mock<ILogger<LoginController>> _mockLogger = new();

    private LoginController CreateController(MockHttpClientFactory fabrica)
        => new(fabrica, _jwtOptions, ProtheusSettingsTeste, _mockLogger.Object);

    public LoginControllerTests()
    {
        _jwtOptions = Options.Create(new JwtSettings
        {
            Secret = "ThisIsAVerySecretKeyForJwtTokenGenerationWithAtLeast256Bits!",
            Issuer = "RedMobileAPI",
            Audience = "RedMobileClients",
            DefaultExpirationInHours = 8
        });
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            NomeUsuario = "testuser",
            Senha = "testpass123"
        };

        var respostaLogin = new RespostaLogin
        {
            Sucesso = true,
            Gerente = false,
            Detalhes = "Login realizado com sucesso"
        };

        var dadosUsuario = new Usuario
        {
            IdRepresentante = "VEND001",
            TipoUsuario = RedMobilePedidos.API.Enums.TipoUsuario.Representante,
            Nome = "Usuário Teste",
            NomeUsuario = "testuser",
            UrlFoto = "http://example.com/photo.jpg"
        };

        var contadorChamadas = 0;
        var mockHandler = new MockHttpMessageHandler((request, cancellationToken) =>
        {
            contadorChamadas++;
            var resposta = contadorChamadas == 1
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(respostaLogin, _jsonOpcoes))
                }
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(dadosUsuario, _jsonOpcoes))
                };
            return Task.FromResult(resposta);
        });

        var mockClient = new HttpClient(mockHandler);
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.Login(loginModel, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var resultadoOk = resultado as OkObjectResult;
        resultadoOk!.Value.Should().BeOfType<TokenModel>();

        var tokenModel = resultadoOk.Value as TokenModel;
        tokenModel!.TokenAcesso.Should().NotBeNullOrEmpty();
        tokenModel.DadosUsuario.Should().NotBeNull();
        tokenModel.DadosUsuario.IdRepresentante.Should().Be("VEND001");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            NomeUsuario = "usuarioinvalido",
            Senha = "senhaerrada"
        };

        var respostaLogin = new RespostaPadrao
        {
            Sucesso = false,
            Detalhes = "Credenciais inválidas"
        };

        var mockClient = MockHttpMessageHandler.CreateMockClient(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(respostaLogin, _jsonOpcoes)
        );
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.Login(loginModel, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<UnauthorizedObjectResult>();
        var resultadoNaoAutorizado = resultado as UnauthorizedObjectResult;
        resultadoNaoAutorizado!.Value.Should().BeOfType<ModeloErro>();
    }

    [Fact]
    public async Task Login_ShouldGenerateValidToken_WhenLoginSucceeds()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            NomeUsuario = "testuser",
            Senha = "testpass123"
        };

        var respostaLogin = new RespostaLogin
        {
            Sucesso = true,
            Gerente = false,
            Detalhes = "Operação realizada com sucesso"
        };
        var dadosUsuario = new Usuario
        {
            IdRepresentante = "VEND001",
            TipoUsuario = RedMobilePedidos.API.Enums.TipoUsuario.Representante,
            Nome = "Usuário Teste",
            NomeUsuario = "testuser",
            UrlFoto = "http://example.com/photo.jpg"
        };

        var contadorChamadas = 0;
        var mockHandler = new MockHttpMessageHandler((request, cancellationToken) =>
        {
            contadorChamadas++;
            var resposta = contadorChamadas == 1
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(respostaLogin, _jsonOpcoes))
                }
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(dadosUsuario, _jsonOpcoes))
                };
            return Task.FromResult(resposta);
        });

        var mockClient = new HttpClient(mockHandler);
        var fabrica = new MockHttpClientFactory(mockClient);
        var controlador = CreateController(fabrica);
        SetupControllerContext(controlador);

        // Act
        var resultado = await controlador.Login(loginModel, CancellationToken.None);

        // Assert
        var resultadoOk = resultado as OkObjectResult;
        var tokenModel = resultadoOk!.Value as TokenModel;
        tokenModel!.TokenAcesso.Should().NotBeNullOrEmpty();
        tokenModel.ExpiraEm.Should().BeAfter(System.DateTime.Now);
        tokenModel.DadosUsuario.IdRepresentante.Should().Be("VEND001");
    }
}
