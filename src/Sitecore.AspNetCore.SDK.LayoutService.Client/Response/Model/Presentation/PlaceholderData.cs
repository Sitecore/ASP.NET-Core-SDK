using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;

/// <summary>
/// Represents the placeholder information for a presentation device returned in a Sitecore layout service response.
/// </summary>
public class PlaceholderData
{
    /// <summary>
    /// Gets or sets the placeholder key.
    /// </summary>
    [DataMember(Name = "key")]
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the instance ID.
    /// </summary>
    [DataMember(Name = "instanceId")]
    [JsonPropertyName("instanceId")]
    public string? InstanceId { get; set; }

    /// <summary>
    /// Gets or sets the metadata ID.
    /// </summary>
    [DataMember(Name = "metadataId")]
    [JsonPropertyName("metadataId")]
    public string? MetadataId { get; set; }
}