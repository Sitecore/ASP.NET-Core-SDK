namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <summary>
/// Represents the page info for a Sitecore site.
/// </summary>
public class PageInfo
{
    /// <summary>
    /// Gets or sets the start cursor.
    /// </summary>
    public string EndCursor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether there are more records to be retrieved.
    /// </summary>
    public bool HasNext { get; set; }
}
