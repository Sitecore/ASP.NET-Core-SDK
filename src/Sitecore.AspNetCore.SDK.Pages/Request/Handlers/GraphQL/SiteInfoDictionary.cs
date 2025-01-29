namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <summary>
/// Represents the dictionary for a Sitecore site.
/// </summary>
public class SiteInfoDictionary
{
    /// <summary>
    /// Gets or sets collection of dictionary items for a Sitecore Site.
    /// </summary>
    public List<SiteInfoDictionaryItem> Results { get; set; } = [];

    /// <summary>
    /// Gets or sets the PageInfo for the Sitecore Site.
    /// </summary>
    public PageInfo? PageInfo { get; set;  }
}