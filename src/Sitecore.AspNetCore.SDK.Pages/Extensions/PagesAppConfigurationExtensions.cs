using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Middleware;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.Pages.Extensions;

/// <summary>
/// Configuration helpers for Pages functionality.
/// </summary>
public static class PagesAppConfigurationExtensions
{
    /// <summary>
    /// Registers the Sitecore Experience Editor middleware into the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="app">The instance of the <see cref="IApplicationBuilder"/> to extend.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseSitecorePages(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        object? experienceEditorMarker = app.ApplicationServices.GetService(typeof(PagesMarkerService));
        if (experienceEditorMarker != null)
        {
            app.UseMiddleware<PagesConfigMiddleware>();
            app.UseMiddleware<PagesRenderMiddleware>();
        }

        return app;
    }

    /// <summary>
    /// Adds the Sitecore Experience Editor support services to the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceBuilder">The <see cref="ISitecoreRenderingEngineBuilder" /> to add services to.</param>
    /// <param name="options">Configures the <see cref="PagesOptions" /> options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static ISitecoreRenderingEngineBuilder WithSitecorePages(this ISitecoreRenderingEngineBuilder serviceBuilder, Action<PagesOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(serviceBuilder);

        IServiceCollection services = serviceBuilder.Services;
        if (services.Any(s => s.ServiceType == typeof(PagesMarkerService)))
        {
            return serviceBuilder;
        }

        services.AddSingleton<PagesMarkerService>();

        if (options != null)
        {
            services.Configure(options);
        }

        services.Configure((Action<RenderingEngineOptions>)(renderingOptions =>
        {
            renderingOptions.MapToRequest((httpRequest, layoutRequest) =>
            {
                MapRequest(httpRequest, layoutRequest, "mode");
                MapRequest(httpRequest, layoutRequest, "sc_itemid");
                MapRequest(httpRequest, layoutRequest, "sc_version");
            });
        }));

        return serviceBuilder;
    }

    private static void MapRequest(HttpRequest httpRequest, SitecoreLayoutRequest layoutRequest, string paramName)
    {
        if (httpRequest.Query == null || !httpRequest.Query.ContainsKey(paramName))
        {
            return;
        }

        string[]? modeQueryValue = httpRequest.Query[paramName];
        if (modeQueryValue == null)
        {
            return;
        }

        layoutRequest.AddHeader(paramName, modeQueryValue);
    }
}
