namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests;

public class HttpLayoutClientMessageHandler : HttpMessageHandler
{
    public Stack<HttpResponseMessage> Responses { get; } = new();

    public List<HttpRequestMessage> Requests { get; } = [];

    public bool WasInvoked { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        WasInvoked = true;
        HttpResponseMessage response = Responses.Pop();
        return Task.FromResult(response);
    }
}