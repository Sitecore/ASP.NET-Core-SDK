using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents chrome information for a Sitecore layout service response.
/// </summary>
public class EditableChrome : IPlaceholderFeature
{
    /// <summary>
    /// Gets or sets the name of the chrome.
    /// </summary>
    [DataMember]
    public string Name { get; set; } = LayoutServiceClientConstants.SitecoreChromes.ChromeTag;

    /// <summary>
    /// Gets or sets the type of the chrome.
    /// </summary>
    public string Type { get; set; } = LayoutServiceClientConstants.SitecoreChromes.ChromeTypeValue;

    /// <summary>
    /// Gets or sets the content of the chrome.
    /// </summary>
    [DataMember(Name = "contents")]
    [JsonPropertyName("contents")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets the attributes for the chrome.
    /// </summary>
    public Dictionary<string, string> Attributes { get; init; } = [];
}