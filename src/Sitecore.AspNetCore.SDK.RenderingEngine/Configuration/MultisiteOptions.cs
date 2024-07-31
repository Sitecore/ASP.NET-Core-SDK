namespace Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;

/// <summary>
/// SitecoreRewriteOptions class represents options for SitecoreRewrite form appsettings.json file.
/// </summary>
public class MultisiteOptions
{
    /// <summary>
    /// Gets Multisite options name.
    /// </summary>
    public const string Name = "Multisite";

    /// <summary>
    /// Gets or sets cache timeout in seconds for site resolving, since it is heavy operation.
    /// </summary>
    public int ResolvingCacheTimeout { get; set; } = 300;
}