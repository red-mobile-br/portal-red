using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RedMobilePedidos.API.Models.Requests;
using RedMobilePedidos.API.Settings;
using RedMobilePedidos.API.Models.Responses.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RedMobilePedidos.API.Controllers;

[Route("api/login")]
public class LoginController(JwtSettings jwtSettings, IHttpClientFactory httpClientFactory, ILogger<LoginController> logger) : BaseApiController(httpClientFactory, logger)
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel, CancellationToken cancellationToken)
    {
        try
        {
            var loginUrl = ConstruirUrlLogin();
            await PublicarAsync<RespostaLogin>(loginModel, loginUrl, cancellationToken);

            var urlDadosUsuario = ConstruirUrlDadosUsuario(loginModel.NomeUsuario);
            var usuario = await ObterAsync<Usuario>(urlDadosUsuario, cancellationToken);
            var usuarioAtualizado = usuario with { NomeUsuario = loginModel.NomeUsuario };
            var modelo = GerarToken(usuarioAtualizado);
            return Ok(modelo);
        }
        catch (Exception ex)
        {
            return ResultadoNaoAutorizado(ex.Message);
        }
    }

    private TokenModel GerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdRepresentante),
            new Claim("tipoUsuario", usuario.TipoUsuario.ToString())
        };

        var segredo = Encoding.UTF8.GetBytes(jwtSettings.Secret);
        var chave = new SymmetricSecurityKey(segredo);
        const string algoritmo = SecurityAlgorithms.HmacSha256;
        var credenciaisAssinatura = new SigningCredentials(chave, algoritmo);
        var inicio = DateTime.Now;
        var expiracao = inicio.AddHours(jwtSettings.DefaultExpirationInHours);

        var tokenJwt = new JwtSecurityToken(
            jwtSettings.Audience,
            jwtSettings.Issuer,
            claims,
            inicio,
            expiracao,
            credenciaisAssinatura);

        var manipuladorToken = new JwtSecurityTokenHandler();
        var tokenAcesso = manipuladorToken.WriteToken(tokenJwt);

        return new TokenModel
        {
            TokenAcesso = tokenAcesso,
            ExpiraEm = expiracao,
            DadosUsuario = usuario
        };
    }

    private static string ConstruirUrlLogin() => string.Join('/', CaminhoApiPadrao, "Login");

    private static string ConstruirUrlDadosUsuario(string nomeUsuario) => string.Join('/', CaminhoApiPadrao, "DadosUsuario", nomeUsuario);
}
