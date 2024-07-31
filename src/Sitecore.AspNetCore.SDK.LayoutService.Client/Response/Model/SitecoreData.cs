using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents the root Sitecore result object returned in a Sitecore layout service response.
/// </summary>
public class SitecoreData
{
    /// <summary>
    /// Gets or sets the <see cref="Context"/> of the layout response.
    /// </summary>
    public Context? Context { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Route"/> of the layout response.
    /// </summary>
    public Route? Route { get; set; }

    /// <summary>
    /// Gets the presentation <see cref="Device"/> list of the layout response.
    /// </summary>
    public List<Device> Devices { get; init; } = [];
}