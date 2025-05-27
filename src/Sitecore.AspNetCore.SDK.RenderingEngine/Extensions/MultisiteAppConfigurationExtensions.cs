using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;
using Sitecore.AspNetCore.SDK.RenderingEngine.Services;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Multisite app configuration.
/// </summary>
public static class MultisiteAppConfigurationExtensions
{
    /// <summary>
    /// Configures multisite. In case of global configuration of RenderingEngine.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns>Modified application builder.</returns>
    public static IApplicationBuilder UseMultisite(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        ISiteResolver? siteResolverService = app.ApplicationServices.GetService<ISiteResolver>();
        ISiteCollectionService? siteCollectionService = app.ApplicationServices.GetService<ISiteCollectionService>();

        if (siteResolverService == null || siteCollectionService == null)
        {
            return app;
        }

        return app.UseMiddleware<MultisiteMiddleware>();
    }

    /// <summary>
    /// Configuration of multisite functionality.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Multisite middleware options configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMultisite(this IServiceCollection services, Action<MultisiteOptions>? configuration = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configuration != null)
        {
            services.Configure(configuration);
        }

        services.TryAddSingleton<ISiteCollectionService, GraphQLSiteCollectionService>();
        services.TryAddSingleton<ISiteResolver, SiteResolver>();

        return services;
    }
}