using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

/// <summary>
/// Supports making requests to the Sitecore layout service.
/// </summary>
public interface ISitecoreLayoutClient : ILayoutRequestHandler
{
    /// <summary>
    /// Invokes a request to the Sitecore layout service using the default handler name.
    /// </summary>
    /// <param name="request">The request details.</param>
    /// <returns>The response of the request.</returns>
    Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request);
}