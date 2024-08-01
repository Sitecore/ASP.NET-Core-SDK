using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;
using Sitecore.AspNetCore.SDK.RenderingEngine.Properties;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Extension methods to support the Microsoft <see cref="IApplicationBuilder"/>.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Registers the Sitecore Rendering Engine middleware into the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="app">The instance of the <see ref="IApplicationBuilder"/> to extend.</param>
    /// <returns>The <see ref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseSitecoreRenderingEngine(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        _ = app.ApplicationServices.GetService(typeof(RenderingEngineMarkerService)) ??
            throw new InvalidOperationException(Resources.Exception_SitecoreRenderingEngineServicesNotRegistered);
        app.UseMiddleware<RenderingEngineMiddleware>();

        return app;
    }

    /// <summary>
    /// Registers the <see cref="HttpRequest"/> to <see cref="SitecoreLayoutRequest"/> action mapping into the <see cref="RenderingEngineOptions"/> object.
    /// </summary>
    /// <param name="app">The instance of the <see cref="IApplicationBuilder"/> to extend.</param>
    /// <param name="mapAction">The mapping action to be added into the options.</param>
    internal static void AddRenderingEngineMapping(this IApplicationBuilder app, Action<HttpRequest, SitecoreLayoutRequest> mapAction)
    {
        IOptions<RenderingEngineOptions> options = app.ApplicationServices.GetRequiredService<IOptions<RenderingEngineOptions>>();

        options.Value.MapToRequest(mapAction);
    }
}