using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProxyKit;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.Providers;

namespace Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;

/// <summary>
/// Sitecore Tracking Visitor identification configurations.
/// </summary>
public static class VisitorIdentificationAppConfigurationExtensions
{
    /// <summary>
    /// Configures application to use sitecore tracking visitor identification. In case of global configuration of RenderingEngine
    /// <see cref="UseSitecoreVisitorIdentification"/> must be executed before <see cref="RenderingEngine.Extensions.ApplicationBuilderExtensions.UseSitecoreRenderingEngine"/>.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns>Modified application builder.</returns>
    public static IApplicationBuilder UseSitecoreVisitorIdentification(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.ApplicationServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)) is not IOptions<SitecoreVisitorIdentificationOptions> options)
        {
            return app;
        }

        if (options.Value.SitecoreInstanceUri != null)
        {
            app.Map("/layouts/System", api =>
            {
                api.RunProxy(async context =>
                {
                    Uri finalUrl = new(options.Value.SitecoreInstanceUri, context.Request.PathBase.ToString());
                    IPAddress? ip = context.Connection.RemoteIpAddress;

                    ForwardContext? forwardContext = context.ForwardTo(finalUrl);
                    forwardContext.UpstreamRequest.Headers.ApplyXForwardedHeaders(ip, context.Request.Host, context.Request.Scheme);

                    return await forwardContext.Send().ConfigureAwait(false);
                });
            });
        }

        return app;
    }

    /// <summary>
    /// Configuration of Sitecore Visitor Identification functionality.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="options">Sitecore Tracking options configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSitecoreVisitorIdentification(this IServiceCollection services, Action<SitecoreVisitorIdentificationOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProxy();

        if (options != null)
        {
            services.Configure(options);
        }

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}