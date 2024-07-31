namespace Sitecore.AspNetCore.SDK.SearchOptimization.Models;

/// <summary>
/// Redirect Info Model.
/// </summary>
internal class RedirectInfo
{
    /// <summary>
    /// Gets or sets the redirect type.
    /// </summary>
    public RedirectType? RedirectType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the query string should be preserved.
    /// </summary>
    public bool IsQueryStringPreserved { get; set; }

    /// <summary>
    /// Gets or sets the pattern.
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    public string? Target { get; set; }
}