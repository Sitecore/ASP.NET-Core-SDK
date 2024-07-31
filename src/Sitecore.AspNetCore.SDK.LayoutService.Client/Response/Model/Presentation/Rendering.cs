using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;

/// <summary>
/// Represents the rendering information for a presentation device returned in a Sitecore layout service response.
/// </summary>
public class Rendering
{
    /// <summary>
    /// Gets or sets the rendering ID.
    /// </summary>
    [DataMember(Name = "id")]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the instance ID.
    /// </summary>
    [DataMember(Name = "instanceId")]
    [JsonPropertyName("instanceId")]
    public string? InstanceId { get; set; }

    /// <summary>
    /// Gets or sets the placeholder key.
    /// </summary>
    [DataMember(Name = "placeholderKey")]
    [JsonPropertyName("placeholderKey")]
    public string PlaceholderKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [DataMember(Name = "dataSource")]
    [JsonPropertyName("dataSource")]
    public string? DataSource { get; set; }

    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    [DataMember(Name = "parameters")]
    [JsonPropertyName("parameters")]
    public Dictionary<string, string> Parameters { get; set; } = [];

    /// <summary>
    /// Gets or sets the caching details.
    /// </summary>
    [DataMember(Name = "caching")]
    [JsonPropertyName("caching")]
    public CachingData? Caching { get; set; }

    /// <summary>
    /// Gets or sets the personalization details.
    /// </summary>
    [DataMember(Name = "analytics")]
    [JsonPropertyName("analytics")]
    public Personalization? Personalization { get; set; }
}