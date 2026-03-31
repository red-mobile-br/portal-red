using System.Net.Http;

namespace RedMobilePedidos.Tests.Helpers;

public class MockHttpClientFactory : IHttpClientFactory
{
    private readonly HttpClient _client;

    public MockHttpClientFactory(HttpClient client)
    {
        _client = client;
    }

    public HttpClient CreateClient(string name)
    {
        return _client;
    }
}
