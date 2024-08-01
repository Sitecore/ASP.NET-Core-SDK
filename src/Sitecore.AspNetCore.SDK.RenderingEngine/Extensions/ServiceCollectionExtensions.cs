using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Filters;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Localization;
using Sitecore.AspNetCore.SDK.RenderingEngine.Mappers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.Routing;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Extension methods for setting up Sitecore Rendering Engine related services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Sitecore Rendering Engine services to the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="options">Configures the Rendering Engine options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static ISitecoreRenderingEngineBuilder AddSitecoreRenderingEngine(
        this IServiceCollection services,
        Action<RenderingEngineOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Only register services if marker interface is missing
        if (services.All(s => s.ServiceType != typeof(RenderingEngineMarkerService)))
        {
            // Always try to add services you don't own
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISitecoreLayoutRequestMapper, SitecoreLayoutRequestMapper>();
            services.AddSingleton<IComponentRendererFactory, ComponentRendererFactory>();
            services.AddSingleton<IEditableChromeRenderer, EditableChromeRenderer>();
            services.AddScoped<IViewModelBinder, SitecoreViewModelBinder>();

            services
                .AddMvc(configure =>
                {
                    configure.Filters.Add(new SitecoreLayoutContextControllerFilter());
                    configure.AddSitecoreModelBinderProviders();
                });

            services.AddLocalizationServices();
            services.AddSingleton<RenderingEngineMarkerService>();
        }

        if (options != null)
        {
            services.Configure(options);
        }

        return new SitecoreRenderingEngineBuilder(services);
    }

    /// <summary>
    /// Enables forwarding of headers.
    /// </summary>
    /// <param name="serviceBuilder">The <see cref="ISitecoreRenderingEngineBuilder" /> to add services to.</param>
    /// <param name="options">Configures the headers forwarding options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static ISitecoreRenderingEngineBuilder ForwardHeaders(this ISitecoreRenderingEngineBuilder serviceBuilder, Action<ForwardHeadersOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(serviceBuilder);

        IServiceCollection services = serviceBuilder.Services;

        if (services.All(s => s.ServiceType != typeof(ForwardHeadersMarkerService)))
        {
            services.Configure<ForwardHeadersOptions>(sitecoreTrackingOptions =>
            {
                // Default white list of headers to transfer to LS
                sitecoreTrackingOptions.RequestHeadersFilters.Add((httpRequest, resultCollection) =>
                {
                    foreach (string headerKey in sitecoreTrackingOptions.HeadersWhitelist)
                    {
                        httpRequest.Headers.CopyHeader(headerKey, resultCollection);
                    }
                });

                sitecoreTrackingOptions.ResponseHeadersFilters.Add((responseMetadata, resultCollection) =>
                {
                    foreach (string headerKey in sitecoreTrackingOptions.HeadersWhitelist)
                    {
                        responseMetadata.CopyHeader(headerKey, resultCollection);
                    }
                });
            });

            services.Configure<RenderingEngineOptions>(renderingOptions =>
            {
                renderingOptions.MapToRequest((httpRequest, layoutRequest) =>
                {
                    ForwardHeadersOptions headersForwardingOptions = httpRequest.HttpContext.RequestServices.GetRequiredService<IOptions<ForwardHeadersOptions>>().Value;

                    IList<Action<HttpRequest, IDictionary<string, string[]>>> headersFilters = headersForwardingOptions.RequestHeadersFilters;

                    Dictionary<string, string[]> proxiedMetadata = new(comparer: StringComparer.OrdinalIgnoreCase);

                    // Apply all filters to headers collection
                    foreach (Action<HttpRequest, IDictionary<string, string[]>> action in headersFilters)
                    {
                        action(httpRequest, proxiedMetadata);
                    }

                    if (!string.IsNullOrEmpty(httpRequest.HttpContext.Request.Scheme))
                    {
                        string scheme = httpRequest.HttpContext.Request.Scheme;

                        proxiedMetadata.Add(headersForwardingOptions.XForwardedProtoHeader, [scheme]);
                    }

                    layoutRequest.AddHeaders(proxiedMetadata);
                });

                renderingOptions.AddPostRenderingAction(httpContext =>
                {
                    ISitecoreRenderingContext? sitecoreRenderingContext = httpContext.GetSitecoreRenderingContext();
                    sitecoreRenderingContext?.UpdateResponseWithLayoutMetadata(httpContext);
                });
            });

            services.AddSingleton<ForwardHeadersMarkerService>();
        }

        if (options != null)
        {
            services.Configure(options);
        }

        return serviceBuilder;
    }

    /// <summary>
    /// Adds sitecore localization services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    internal static void AddLocalizationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<SitecoreQueryStringCultureProvider>();
        services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add(RenderingEngineConstants.SitecoreLocalization.RequestCulturePrefix, typeof(LanguageRouteConstraint));
        });
    }
}