using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Mappers;

/// <summary>
/// Class that maps the layout response according to the options.
/// </summary>
internal class SitecoreLayoutResponseMapper
{
    private readonly ICollection<Action<SitecoreLayoutResponseContent, string, HttpRequest>> _handlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutResponseMapper"/> class.
    /// </summary>
    /// <param name="options">The <see cref="ExperienceEditorOptions"/> instance.</param>
    public SitecoreLayoutResponseMapper(IOptions<ExperienceEditorOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _handlers = options.Value.ItemMappings;
    }

    /// <summary>
    /// Maps the route to a request path.
    /// </summary>
    /// <param name="response">Layout Response.</param>
    /// <param name="scPath">Sitecore Path.</param>
    /// <param name="request">Request data.</param>
    /// <returns>Path of the request.</returns>
    public string? MapRoute(SitecoreLayoutResponseContent response, string scPath, HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentException.ThrowIfNullOrWhiteSpace(scPath);
        ArgumentNullException.ThrowIfNull(request);

        if (_handlers.Count == 0)
        {
            return scPath;
        }

        foreach (Action<SitecoreLayoutResponseContent, string, HttpRequest> handler in _handlers)
        {
            handler.Invoke(response, scPath, request);
        }

        return request.Path.Value;
    }
}