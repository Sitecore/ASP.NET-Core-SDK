using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Properties;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Mappers;

/// <summary>
/// Maps a <see cref="HttpRequest"/> to a <see cref="SitecoreLayoutRequest"/>.
/// </summary>
public class SitecoreLayoutRequestMapper : ISitecoreLayoutRequestMapper
{
    private readonly ICollection<Action<HttpRequest, SitecoreLayoutRequest>> _handlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutRequestMapper"/> class.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> instance.</param>
    public SitecoreLayoutRequestMapper(IOptions<RenderingEngineOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _handlers = options.Value.RequestMappings;
        if (_handlers == null)
        {
            throw new ArgumentException(Resources.Exception_RequestMappings_Required);
        }
    }

    /// <inheritdoc />
    public SitecoreLayoutRequest Map(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        SitecoreLayoutRequest scRequest = [];

        foreach (Action<HttpRequest, SitecoreLayoutRequest> handler in _handlers)
        {
            handler.Invoke(request, scRequest);
        }

        return scRequest;
    }
}