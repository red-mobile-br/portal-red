using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RedMobilePedidos.API.Settings;

namespace RedMobilePedidos.Tests.Controllers;

public abstract class BaseControllerTests
{
    protected const string TestUserId = "TEST_USER_001";

    protected static readonly IOptions<ProtheusSettings> ProtheusSettingsTeste = Options.Create(new ProtheusSettings
    {
        BaseUrl = "http://protheus-teste",
        Usuario = "usuario-teste",
        Senha = "senha-teste"
    });

    protected static void SetupControllerContext(ControllerBase controller, string userId = TestUserId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }
}
