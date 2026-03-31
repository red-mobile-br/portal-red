namespace RedMobilePedidos.API.Services;

public interface IEmailTemplateService
{
    string RenderizarTemplate(string nomeTemplate, Dictionary<string, string> variaveis);
    string RenderizarTemplate<T>(string nomeTemplate, T modelo) where T : class;
    string RenderizarTemplate<T>(string pasta, string nomeTemplate, T modelo) where T : class;
}
