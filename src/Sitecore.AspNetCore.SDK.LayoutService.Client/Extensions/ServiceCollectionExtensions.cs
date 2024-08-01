using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

/// <summary>
/// Extension methods to support Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required to support the Sitecore layout service to the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">>An action to configure the <see cref="SitecoreLayoutClientOptions"/>.</param>
    /// <returns>An <see cref="ISitecoreLayoutClientBuilder"/> so that Sitecore layout services may be configured further.</returns>
    public static ISitecoreLayoutClientBuilder AddSitecoreLayoutService(
        this IServiceCollection services,
        Action<SitecoreLayoutClientOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Only register services if marker interface is missing
        if (services.All(s => s.ServiceType != typeof(SitecoreLayoutServiceMarkerService)))
        {
            services.AddTransient<ISitecoreLayoutClient>(
                sp =>
                {
                    using IServiceScope scope = sp.CreateScope();
                    return ActivatorUtilities.CreateInstance<DefaultLayoutClient>(scope.ServiceProvider, sp);
                });

            SetSerializer(services);
        }

        if (options != null)
        {
            services.Configure(options);
        }

        return new SitecoreLayoutClientBuilder(services);
    }

    private static void SetSerializer(IServiceCollection services)
    {
        services.AddSingleton<ISitecoreLayoutSerializer>(new JsonLayoutServiceSerializer());
    }
}