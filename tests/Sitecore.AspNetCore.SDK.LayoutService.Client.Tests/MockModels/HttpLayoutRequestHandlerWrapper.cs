using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.MockModels;

public class HttpLayoutRequestHandlerWrapper : HttpLayoutRequestHandler
{
    public HttpLayoutRequestHandlerWrapper(HttpClient client, ISitecoreLayoutSerializer serializer, IOptionsSnapshot<HttpLayoutRequestHandlerOptions> options, ILogger<HttpLayoutRequestHandler> logger)
        : base(client, serializer, options, logger)
    {
        // Add a default success response
        Responses.Push(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("JSON")
        });
    }

    public HttpRequestMessage? RequestMessage { get; private set; }

    public Stack<HttpResponseMessage> Responses { get; } = new();

    protected override Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage requestMessage)
    {
        RequestMessage = requestMessage;

        HttpResponseMessage response = Responses.Pop();

        response.RequestMessage = requestMessage;

        return Task.FromResult(response);
    }
}