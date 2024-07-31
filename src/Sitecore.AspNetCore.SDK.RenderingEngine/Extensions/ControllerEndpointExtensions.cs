using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Contains extension methods for using Controllers with <see cref="IEndpointRouteBuilder"/>.
/// </summary>
public static class ControllerEndpointExtensions
{
    private const string Pattern = $"{{{RenderingEngineConstants.SitecoreLocalization.RequestCulturePrefix}:{RenderingEngineConstants.SitecoreLocalization.RequestCulturePrefix}}}";

    /// <summary>
    /// Add default endpoint which supports language embedded prefixes. Like Http://localhost/da-DK/home.
    /// </summary>
    /// <param name="endpoints"> <see cref="IEndpointRouteBuilder"/> parameter.</param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="action">Default action.</param>
    /// <param name="controller">Default controller.</param>
    /// <returns>A <see cref="ControllerActionEndpointConventionBuilder"/> for endpoints associated with controller actions for this route.</returns>
    public static IEndpointConventionBuilder MapSitecoreLocalizedRoute(this IEndpointRouteBuilder endpoints, string routeName, string action, string controller)
    {
        ArgumentNullException.ThrowIfNull(routeName);
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        return endpoints.MapControllerRoute(
            name: routeName,
            pattern: $"/{Pattern}/{{**{RenderingEngineConstants.RouteValues.SitecoreRoute}}}",
            defaults: new { controller, action });
    }
}