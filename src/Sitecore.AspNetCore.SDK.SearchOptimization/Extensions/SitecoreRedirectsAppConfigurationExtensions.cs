using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.SearchOptimization.Redirects.Middleware;
using Sitecore.AspNetCore.SDK.SearchOptimization.Services;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Extensions;

/// <summary>
/// Sitecore redirects configuration.
/// </summary>
public static class SitecoreRedirectsAppConfigurationExtensions
{
    /// <summary>
    /// Configures Sitecore redirects. In case of global configuration of RenderingEngine.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <param name="staticRewriteOptions">Static rewrite options that will be merged with Sitecore rewrite options.</param>
    /// <returns>Modified application builder.</returns>
    public static IApplicationBuilder UseSitecoreRedirects(this IApplicationBuilder app, RewriteOptions? staticRewriteOptions = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (staticRewriteOptions is null)
        {
            return app.UseMiddleware<SitecoreRewriteMiddleware>();
        }

        return app.UseMiddleware<SitecoreRewriteMiddleware>(Options.Create(staticRewriteOptions));
    }

    /// <summary>
    /// Configuration of Sitecore redirects functionality.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSitecoreRedirects(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IRedirectsService, GraphQlSiteInfoService>();

        return services;
    }
}