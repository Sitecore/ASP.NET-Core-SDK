using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

/// <summary>
/// Models the content of the response from calling the Sitecore layout service.
/// </summary>
public class SitecoreLayoutResponseContent
{
    /// <summary>
    /// Gets or sets the root <see cref="SitecoreData"/> object.
    /// </summary>
    public SitecoreData? Sitecore { get; set; }

    /// <summary>
    /// Gets or sets  <see cref="ContextRawData"/> string.
    /// </summary>
    public string ContextRawData { get; set; } = string.Empty;
}