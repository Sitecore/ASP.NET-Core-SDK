namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <summary>
/// Represents a dictionary item for a Sitecore site.
/// </summary>
public class SiteInfoDictionaryItem
{
    /// <summary>
    /// Gets or sets the key for the dictionary item.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value for the dictionary item.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
