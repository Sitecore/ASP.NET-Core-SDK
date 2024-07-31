using Microsoft.AspNetCore.Http;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

/// <summary>
/// Contract for implementing the mapping logic from a <see cref="HttpRequest"/> to a <see cref="SitecoreLayoutRequest"/>.
/// </summary>
public interface ISitecoreLayoutRequestMapper
{
    /// <summary>
    /// Maps a <see cref="HttpRequest"/> to a <see cref="SitecoreLayoutRequest"/>.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/> to map.</param>
    /// <returns>A mapped <see cref="SitecoreLayoutRequest"/>.</returns>
    SitecoreLayoutRequest Map(HttpRequest request);
}