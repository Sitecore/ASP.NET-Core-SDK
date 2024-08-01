using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents the context of a Sitecore layout response.
/// </summary>
public class Context
{
    /// <summary>
    /// Gets or sets a value indicating whether the page is in editing mode.
    /// </summary>
    [DataMember(Name = "pageEditing")]
    [JsonPropertyName("pageEditing")]
    public bool IsEditing { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Site"/> of the response.
    /// </summary>
    public Site? Site { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="PageState"/> of the response.
    /// </summary>
    public PageState? PageState { get; set; }

    /// <summary>
    /// Gets or sets the language of the response.
    /// </summary>
    public string Language { get; set; } = string.Empty;
}