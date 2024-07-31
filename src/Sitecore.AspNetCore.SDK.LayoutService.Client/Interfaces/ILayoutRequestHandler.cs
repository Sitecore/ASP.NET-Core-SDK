using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

/// <summary>
/// Supports making requests to the Sitecore layout service.
/// </summary>
public interface ILayoutRequestHandler
{
    /// <summary>
    /// Handles a request to the Sitecore layout service using the specified handler.
    /// </summary>
    /// <param name="request">The request details.</param>
    /// <param name="handlerName">The name of the request handler to use to handle the request.</param>
    /// <returns>The response of the request.</returns>
    Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName);
}