using HandlebarsDotNet;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RedMobilePedidos.API.Services;

internal partial class EmailTemplateService(IMemoryCache cache) : IEmailTemplateService
{
    private readonly IHandlebars _handlebars = CriarInstanciaHandlebars();
    private readonly string _caminhoTemplates = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
    private static readonly TimeSpan DuracaoCache = TimeSpan.FromHours(24);
    private static readonly CultureInfo CulturaPtBr = new("pt-BR");

    public string RenderizarTemplate(string nomeTemplate, Dictionary<string, string> variaveis)
    {
        var templateCompilado = ObterTemplateCompilado("Email", nomeTemplate);
        return templateCompilado(variaveis);
    }

    /// <summary>
    /// Renderiza um template com um modelo fortemente tipado para melhor desempenho.
    /// </summary>
    public string RenderizarTemplate<T>(string nomeTemplate, T modelo) where T : class
    {
        var templateCompilado = ObterTemplateCompilado("Email", nomeTemplate);
        return templateCompilado(modelo);
    }

    /// <summary>
    /// Renderiza um template de uma pasta específica com um modelo fortemente tipado.
    /// </summary>
    public string RenderizarTemplate<T>(string pasta, string nomeTemplate, T modelo) where T : class
    {
        var templateCompilado = ObterTemplateCompilado(pasta, nomeTemplate);
        return templateCompilado(modelo);
    }

    private HandlebarsTemplate<object, object> ObterTemplateCompilado(string pasta, string nomeTemplate)
    {
        var chaveCache = $"handlebars_template_{pasta}_{nomeTemplate}";

        return cache.GetOrCreate(chaveCache, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = DuracaoCache;

            var caminhoArquivo = Path.Combine(_caminhoTemplates, pasta, $"{nomeTemplate}.html");

            if (!File.Exists(caminhoArquivo))
            {
                throw new FileNotFoundException($"Template '{nomeTemplate}' não encontrado em: {caminhoArquivo}");
            }

            var fonteTemplate = File.ReadAllText(caminhoArquivo);
            return _handlebars.Compile(fonteTemplate);
        })!;
    }

    private static IHandlebars CriarInstanciaHandlebars()
    {
        var handlebars = Handlebars.Create();

        // Helper para formatar moeda em Real brasileiro
        handlebars.RegisterHelper("currency", (writer, _, parameters) =>
        {
            if (parameters.Length > 0 && parameters[0] != null)
            {
                writer.WriteSafeString(decimal.TryParse(parameters[0].ToString(), out var valor)
                    ? valor.ToString("C", CulturaPtBr)
                    : parameters[0].ToString());
            }
        });

        // Helper para formatar data
        handlebars.RegisterHelper("formatarData", (writer, _, parameters) =>
        {
            if (parameters.Length > 0 && parameters[0] != null)
            {
                var formato = parameters.Length > 1 ? parameters[1]?.ToString() ?? "dd/MM/yyyy" : "dd/MM/yyyy";

                switch (parameters[0])
                {
                    case DateTime data:
                        writer.WriteSafeString(data.ToString(formato, CulturaPtBr));
                        break;

                    default:
                        writer.WriteSafeString(DateTime.TryParse(parameters[0].ToString(), out var dataConvertida)
                            ? dataConvertida.ToString(formato, CulturaPtBr)
                            : parameters[0].ToString());
                        break;
                }
            }
        });

        // Helper para comparação de igualdade (útil para condicionais)
        handlebars.RegisterHelper("eq", (output, options, context, arguments) =>
        {
            if (arguments.Length >= 2)
            {
                var esquerda = arguments[0]?.ToString();
                var direita = arguments[1]?.ToString();

                if (esquerda == direita)
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            }
        });

        // Helper para verificar se valor é maior que zero
        handlebars.RegisterHelper("sePositivo", (output, options, context, arguments) =>
        {
            if (arguments.Length > 0 && decimal.TryParse(arguments[0]?.ToString(), out var valor) && valor > 0)
            {
                options.Template(output, context);
            }
            else
            {
                options.Inverse(output, context);
            }
        });

        // Helper para formatar decimal (sem símbolo de moeda)
        handlebars.RegisterHelper("formatarDecimal", (writer, _, parameters) =>
        {
            if (parameters.Length > 0 && parameters[0] != null)
            {
                writer.WriteSafeString(decimal.TryParse(parameters[0].ToString(), out var valor)
                    ? valor.ToString("N2", CulturaPtBr)
                    : parameters[0].ToString());
            }
        });

        // Helper para formatar CEP (XXXXX-XXX)
        handlebars.RegisterHelper("formatarCep", (writer, _, parameters) =>
        {
            if (parameters.Length > 0 && parameters[0] != null)
            {
                var valor = parameters[0].ToString()?.Replace("-", "") ?? "";
                writer.WriteSafeString(valor.Length >= 8
                    ? $"{valor[..5]}-{valor[5..8]}"
                    : valor);
            }
        });

        // Helper para formatar código de cliente (XXXXXX-XX)
        handlebars.RegisterHelper("formatarCodigoCliente", (writer, _, parameters) =>
        {
            if (parameters.Length > 0 && parameters[0] != null)
            {
                var valor = RegexSomenteNumeros().Replace(parameters[0].ToString() ?? "", "");
                writer.WriteSafeString(valor.Length >= 8
                    ? $"{valor[..6]}-{valor[6..8]}"
                    : valor);
            }
        });

        return handlebars;
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex RegexSomenteNumeros();
}
