namespace Sitecore.AspNetCore.SDK.AutoFixture.Mocks;

public class MockHttpMessageHandler : HttpMessageHandler
{
    public Stack<HttpResponseMessage> Responses { get; } = new();

    public List<HttpRequestMessage> Requests { get; } = [];

    public bool WasInvoked { get; private set; }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        WasInvoked = true;
        HttpResponseMessage response = Responses.Pop();
        return response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        WasInvoked = true;
        HttpResponseMessage response = Responses.Pop();
        return Task.FromResult(response);
    }
}