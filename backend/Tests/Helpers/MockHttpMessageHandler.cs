using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RedMobilePedidos.Tests.Helpers;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;

    public MockHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
    {
        _sendAsync = sendAsync;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _sendAsync(request, cancellationToken);
    }

    public static HttpClient CreateMockClient(HttpStatusCode statusCode, string content)
    {
        var mockHandler = new MockHttpMessageHandler((request, cancellationToken) =>
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            };
            return Task.FromResult(response);
        });

        return new HttpClient(mockHandler);
    }

    public static HttpClient CreateMockClient()
    {
        return CreateMockClient(HttpStatusCode.OK, string.Empty);
    }
}
