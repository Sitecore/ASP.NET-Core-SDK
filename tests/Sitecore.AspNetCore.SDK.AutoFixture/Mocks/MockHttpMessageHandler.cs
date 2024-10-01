using System.Diagnostics.CodeAnalysis;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;

namespace Sitecore.AspNetCore.SDK.AutoFixture.Mocks;

[ExcludeFromCodeCoverage]
public class MockHttpMessageHandler : HttpMessageHandler
{
    public Stack<HttpResponseMessage> Responses { get; } = new();

    public List<HttpRequestMessage> Requests { get; } = [];

    public bool WasInvoked { get; private set; }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Task<HttpRequestMessage> clone = request.Clone();
        clone.Wait(cancellationToken);
        Requests.Add(clone.Result);
        WasInvoked = true;
        HttpResponseMessage response = Responses.Pop();
        return response;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(await request.Clone().WaitAsync(cancellationToken));
        WasInvoked = true;
        HttpResponseMessage response = Responses.Pop();
        return response;
    }
}