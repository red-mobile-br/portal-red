using Microsoft.Extensions.Options;
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
public class LoginController(IHttpClientFactory httpClientFactory, IOptions<JwtSettings> jwtOptions, IOptions<ProtheusSettings> protheusOptions, ILogger<LoginController> logger) : BaseApiController(httpClientFactory, protheusOptions, logger)
{
    private JwtSettings Jwt => jwtOptions.Value;

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

        var segredo = Encoding.UTF8.GetBytes(Jwt.Secret);
        var chave = new SymmetricSecurityKey(segredo);
        const string algoritmo = SecurityAlgorithms.HmacSha256;
        var credenciaisAssinatura = new SigningCredentials(chave, algoritmo);
        var inicio = DateTime.Now;
        var expiracao = inicio.AddHours(Jwt.DefaultExpirationInHours);

        var tokenJwt = new JwtSecurityToken(
            Jwt.Audience,
            Jwt.Issuer,
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

    private string ConstruirUrlLogin() => string.Join('/', CaminhoApiPadrao, "Login");

    private string ConstruirUrlDadosUsuario(string nomeUsuario) => string.Join('/', CaminhoApiPadrao, "DadosUsuario", nomeUsuario);
}
