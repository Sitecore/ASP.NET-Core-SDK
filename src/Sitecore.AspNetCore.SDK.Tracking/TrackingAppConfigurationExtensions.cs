using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.Tracking;

/// <summary>
/// Sitecore Tracking configurations.
/// </summary>
public static class TrackingAppConfigurationExtensions
{
    /// <summary>
    /// Configuration of Sitecore tracking functionality.
    /// </summary>
    /// <param name="serviceBuilder">The <see cref="ISitecoreRenderingEngineBuilder" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static ISitecoreRenderingEngineBuilder WithTracking(this ISitecoreRenderingEngineBuilder serviceBuilder)
    {
        ArgumentNullException.ThrowIfNull(serviceBuilder);

        serviceBuilder.ForwardHeaders(o =>
        {
            o.HeadersWhitelist.Add("cookie");
            o.HeadersWhitelist.Add("set-cookie");
            o.HeadersWhitelist.Add("user-agent");
            o.HeadersWhitelist.Add("referer");
        });

        serviceBuilder.Services.Configure<RenderingEngineOptions>(renderingOptions =>
        {
            renderingOptions.MapToRequest((httpRequest, layoutRequest) =>
            {
                Dictionary<string, string[]> proxiedMetadata = new(StringComparer.OrdinalIgnoreCase);

                if (httpRequest.HttpContext.Connection.RemoteIpAddress != null)
                {
                    string ip = httpRequest.HttpContext.Connection.RemoteIpAddress.ToString();

                    proxiedMetadata.Add("X-Forwarded-For", [ip]);
                }

                layoutRequest.AddHeaders(proxiedMetadata);
            });
        });

        return serviceBuilder;
    }
}