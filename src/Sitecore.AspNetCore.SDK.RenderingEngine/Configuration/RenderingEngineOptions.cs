using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;

/// <summary>
/// The options to configure the Sitecore Rendering Engine.
/// </summary>
public class RenderingEngineOptions
{
#pragma warning disable SA1513 // Closing brace should be followed by blank line
    /// <summary>
    /// Gets or sets the action list to configure the <see cref="SitecoreLayoutRequest"/> handler for incoming HTTP requests.
    /// </summary>
    public ICollection<Action<HttpRequest, SitecoreLayoutRequest>> RequestMappings { get; set; } =
    [
        (http, sc) =>
        {
            sc.Path($"/{GetRequestPath(http)}");

            IRequestCultureFeature? feature = http.HttpContext.Features.Get<IRequestCultureFeature>();
            if (feature?.RequestCulture.Culture != null)
            {
                sc.Language(feature.RequestCulture.Culture.Name);
            }
        }
    ];
#pragma warning restore SA1513 // Closing brace should be followed by blank line

    /// <summary>
    /// Gets or sets the list of <see cref="ComponentRendererDescriptor"/> objects for handling component rendering.
    /// </summary>
    public SortedList<int, ComponentRendererDescriptor> RendererRegistry { get; set; } = [];

    /// <summary>
    /// Gets or sets the default <see cref="ComponentRendererDescriptor"/> object for handling component rendering.
    /// </summary>
    public ComponentRendererDescriptor? DefaultRenderer { get; set; }

    /// <summary>
    /// Gets collection of actions to be executed right after RenderingEngine middleware logic.
    /// </summary>
    public ICollection<Action<HttpContext>> PostRenderingActions { get; } = [];

    private static string GetRequestPath(HttpRequest http)
    {
        string? path;

        if (http.RouteValues.ContainsKey(RenderingEngineConstants.RouteValues.SitecoreRoute))
        {
            path = http.RouteValues[RenderingEngineConstants.RouteValues.SitecoreRoute] != null ?
                http.RouteValues[RenderingEngineConstants.RouteValues.SitecoreRoute]!.ToString() :
                string.Empty;
        }
        else
        {
            path = http.Path.Value;
        }

        return path != null ? path.TrimStart('/') : string.Empty;
    }
}