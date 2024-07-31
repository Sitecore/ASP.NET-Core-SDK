namespace Sitecore.AspNetCore.SDK.SearchOptimization.Models;

/// <summary>
/// Site Info Model.
/// </summary>
internal class SiteInfo
{
    /// <summary>
    /// Gets or sets the site map.
    /// </summary>
    public string[]? Sitemap { get; set; }

    /// <summary>
    /// Gets or sets the redirects.
    /// </summary>
    public RedirectInfo[]? Redirects { get; set; }
}