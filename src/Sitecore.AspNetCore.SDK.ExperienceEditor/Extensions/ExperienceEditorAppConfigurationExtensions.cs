using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Middleware;
using Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;

/// <summary>
/// Configuration helpers for ExperienceEditor functionality.
/// </summary>
public static class ExperienceEditorAppConfigurationExtensions
{
    /// <summary>
    /// Registers the Sitecore Experience Editor middleware into the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="app">The instance of the <see cref="IApplicationBuilder"/> to extend.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseSitecoreExperienceEditor(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        object? experienceEditorMarker = app.ApplicationServices.GetService(typeof(ExperienceEditorMarkerService));
        if (experienceEditorMarker != null)
        {
            app.UseMiddleware<ExperienceEditorMiddleware>();
        }

        return app;
    }

    /// <summary>
    /// Adds the Sitecore Experience Editor support services to the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceBuilder">The <see cref="ISitecoreRenderingEngineBuilder" /> to add services to.</param>
    /// <param name="options">Configures the <see cref="ExperienceEditorOptions" /> options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static ISitecoreRenderingEngineBuilder WithExperienceEditor(this ISitecoreRenderingEngineBuilder serviceBuilder, Action<ExperienceEditorOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(serviceBuilder);

        IServiceCollection services = serviceBuilder.Services;
        if (services.Any(s => s.ServiceType == typeof(ExperienceEditorMarkerService)))
        {
            return serviceBuilder;
        }

        services.AddSingleton<IChromeDataSerializer, ChromeDataSerializer>();
        services.AddSingleton<IChromeDataBuilder, ChromeDataBuilder>();
        services.AddSingleton<ExperienceEditorMarkerService>();

        if (options != null)
        {
            services.Configure(options);
        }

        return serviceBuilder;
    }
}