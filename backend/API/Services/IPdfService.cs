namespace RedMobilePedidos.API.Services;

public interface IPdfService
{
    Task<byte[]> GerarPdfDeHtmlAsync(string html);
}
