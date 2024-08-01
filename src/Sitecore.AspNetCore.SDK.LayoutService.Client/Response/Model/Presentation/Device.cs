using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;

/// <summary>
/// Represents a presentation device returned in a Sitecore layout service response.
/// </summary>
public class Device
{
    /// <summary>
    /// Gets or sets the device ID.
    /// </summary>
    [DataMember(Name = "id")]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the device layout ID.
    /// </summary>
    [DataMember(Name = "layoutId")]
    [JsonPropertyName("layoutId")]
    public string? LayoutId { get; set; }

    /// <summary>
    /// Gets or sets the list of placeholder details.
    /// </summary>
    [DataMember(Name = "placeholders")]
    [JsonPropertyName("placeholders")]
    public List<PlaceholderData> Placeholders { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of renderings.
    /// </summary>
    [DataMember(Name = "renderings")]
    [JsonPropertyName("renderings")]
    public List<Rendering> Renderings { get; set; } = [];
}