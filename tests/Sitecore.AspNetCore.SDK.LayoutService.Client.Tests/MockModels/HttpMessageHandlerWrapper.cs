namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.MockModels;

public class HttpMessageHandlerWrapper : HttpMessageHandler
{
    public List<HttpRequestMessage> Messages { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Messages.Add(request);
        return Task.FromResult(new HttpResponseMessage());
    }
}