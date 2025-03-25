namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <summary>
/// Represents a Sitecore site.
/// </summary>
public class Site
{
    /// <summary>
    /// Gets or sets the site info.
    /// </summary>
    public SiteInfo SiteInfo { get; set; } = new();
}
