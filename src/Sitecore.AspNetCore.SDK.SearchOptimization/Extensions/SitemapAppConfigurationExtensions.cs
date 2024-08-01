using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProxyKit;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;
using Sitecore.AspNetCore.SDK.SearchOptimization.Services;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Extensions;

/// <summary>
/// Sitemap configuration.
/// </summary>
public static partial class SitemapAppConfigurationExtensions
{
    /// <summary>
    /// Configures sitemap. In case of global configuration of RenderingEngine.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns>Modified application builder.</returns>
    public static IApplicationBuilder UseSitemap(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        ISitemapService? sitemapService = app.ApplicationServices.GetService<ISitemapService>();

        if (sitemapService == null)
        {
            return app;
        }

        app.MapWhen(
            context =>
            {
                if (context.Request.Path.HasValue)
                {
                    return SitemapRegex().Match(context.Request.Path.Value).Success;
                }

                return false;
            },
            api =>
            {
                api.RunProxy(async context =>
                {
                    if (context.Request.Path.HasValue)
                    {
                        context.TryGetResolvedSiteName(out string? resolvedSiteName);

                        string url = await sitemapService.GetSitemapUrl(context.Request.Path.Value, resolvedSiteName).ConfigureAwait(false);

                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            context.Request.Path = string.Empty;
                            ForwardContext? forwardContext = context.ForwardTo(url);
                            return await forwardContext.Send().ConfigureAwait(false);
                        }
                    }

                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                });
            });

        return app;
    }

    /// <summary>
    /// Configuration of Edge sitemap functionality, which is using GraphQL.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddEdgeSitemap(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProxy();

        services.TryAddSingleton<ISitemapService, GraphQlSiteInfoService>();

        return services;
    }

    /// <summary>
    /// Configuration of Sitemap functionality, which is proxies to specified instance Uri.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The <see cref="SitemapOptions" /> configuration needed to resolve sitemap.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSitemap(this IServiceCollection services, Action<SitemapOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddProxy();

        services.Configure(configuration);

        services.AddSingleton<ISitemapService, SitemapService>();

        return services;
    }

    [GeneratedRegex("^/sitemap(-[0-9]{1,2})?.xml$")]
    private static partial Regex SitemapRegex();
}