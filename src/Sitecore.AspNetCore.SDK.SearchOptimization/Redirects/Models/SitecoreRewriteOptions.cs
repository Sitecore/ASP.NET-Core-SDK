namespace Sitecore.AspNetCore.SDK.SearchOptimization.Redirects.Models;

/// <summary>
/// SitecoreRewriteOptions class represents options for SitecoreRewrite form appsettings.json file.
/// </summary>
public class SitecoreRewriteOptions
{
    /// <summary>
    /// Gets SitecoreRewrite options name.
    /// </summary>
    public const string Name = "SitecoreRewrite";

    /// <summary>
    /// Gets or sets cache timeout in seconds for getting redirects from the sitecore.
    /// </summary>
    public int CacheTimeout { get; set; } = 60;
}