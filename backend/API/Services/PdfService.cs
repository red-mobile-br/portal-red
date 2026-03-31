using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace RedMobilePedidos.API.Services;

internal sealed partial class PdfService(ILogger<PdfService> logger) : IPdfService
{
    private static readonly TimeSpan TimeoutNavegador = TimeSpan.FromSeconds(30);

    public async Task<byte[]> GerarPdfDeHtmlAsync(string html)
    {
        try
        {
            var caminhoExecutavel = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");
            var eDocker = File.Exists("/.dockerenv") || Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            var args = new List<string>
            {
                "--no-sandbox",
                "--disable-setuid-sandbox",
                "--disable-dev-shm-usage",
                "--disable-gpu",
                "--disable-software-rasterizer",
                "--disable-extensions",
                "--disable-background-networking",
                "--disable-default-apps",
                "--disable-sync",
                "--disable-translate",
                "--hide-scrollbars",
                "--mute-audio",
                "--no-first-run",
                "--safebrowsing-disable-auto-update"
            };

            if (eDocker)
            {
                args.Add("--no-zygote");
            }

            var opcoesInicio = new LaunchOptions
            {
                Headless = true,
                Args = args.ToArray(),
                Timeout = (int)TimeoutNavegador.TotalMilliseconds
            };

            if (!string.IsNullOrEmpty(caminhoExecutavel))
            {
                opcoesInicio.ExecutablePath = caminhoExecutavel;
            }
            else
            {
                var baixadorNavegador = new BrowserFetcher();
                await baixadorNavegador.DownloadAsync();
            }

            await using var navegador = await Puppeteer.LaunchAsync(opcoesInicio);
            await using var pagina = await navegador.NewPageAsync();

            pagina.DefaultTimeout = (int)TimeoutNavegador.TotalMilliseconds;
            pagina.DefaultNavigationTimeout = (int)TimeoutNavegador.TotalMilliseconds;

            await pagina.SetContentAsync(html, new NavigationOptions
            {
                WaitUntil = [WaitUntilNavigation.Load],
                Timeout = (int)TimeoutNavegador.TotalMilliseconds
            });

            var bytesPdf = await pagina.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "0.5cm",
                    Right = "0.5cm",
                    Bottom = "0.5cm",
                    Left = "0.5cm"
                }
            });

            LogPdfGeradoComSucesso(logger, bytesPdf.Length);

            return bytesPdf;
        }
        catch (TimeoutException ex)
        {
            LogOperacaoNavegadorExpirou(logger, ex);
            throw;
        }
        catch (ProcessException ex)
        {
            LogProcessoNavegadorFalhouAoIniciar(logger, ex);
            throw;
        }
        catch (Exception ex)
        {
            LogFalhaAoGerarPdfDeHtml(logger, ex);
            throw;
        }
    }
}
