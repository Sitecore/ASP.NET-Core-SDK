namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <summary>
/// Represents the info for a Sitecore site.
/// </summary>
public class SiteInfo
{
    /// <summary>
    /// Gets or sets the dictionary for a Sitecore Site.
    /// </summary>
    public SiteInfoDictionary Dictionary { get; set; } = new();
}
