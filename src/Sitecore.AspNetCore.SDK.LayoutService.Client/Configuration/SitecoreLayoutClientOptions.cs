using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;

/// <summary>
/// Options to control the Sitecore <see cref="DefaultLayoutClient"/>.
/// </summary>
public class SitecoreLayoutClientOptions
{
    /// <summary>
    /// Gets the registry of Sitecore layout service request handlers.
    /// </summary>
    public Dictionary<string, Func<IServiceProvider, ILayoutRequestHandler>> HandlerRegistry { get; init; } = [];

    /// <summary>
    /// Gets or sets the default handler name for requests.
    /// </summary>
    public string? DefaultHandler { get; set; }
}