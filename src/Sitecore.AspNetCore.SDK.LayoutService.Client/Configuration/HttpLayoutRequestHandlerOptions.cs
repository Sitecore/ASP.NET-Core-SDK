using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;

/// <summary>
/// Options to control the <see cref="HttpLayoutRequestHandler"/> for the Sitecore layout service.
/// </summary>
public class HttpLayoutRequestHandlerOptions : IMapRequest<HttpRequestMessage>
{
    /// <inheritdoc />
    public List<Action<SitecoreLayoutRequest, HttpRequestMessage>> RequestMap { get; init; } = [];
}