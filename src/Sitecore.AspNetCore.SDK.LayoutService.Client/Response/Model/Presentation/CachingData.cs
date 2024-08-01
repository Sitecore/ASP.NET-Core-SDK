using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;

/// <summary>
/// Represents the caching information for a device rendering returned in a Sitecore layout service response.
/// </summary>
public class CachingData
{
    /// <summary>
    /// Gets or sets the 'cacheable' flag.
    /// </summary>
    [DataMember(Name = "cacheable")]
    [JsonPropertyName("cacheable")]
    public bool? Cacheable { get; set; }

    /// <summary>
    /// Gets or sets the 'vary by data' flag.
    /// </summary>
    [DataMember(Name = "varyByData")]
    [JsonPropertyName("varyByData")]
    public bool? VaryByData { get; set; }

    /// <summary>
    /// Gets or sets the 'vary by device' flag.
    /// </summary>
    [DataMember(Name = "varyByDevice")]
    [JsonPropertyName("varyByDevice")]
    public bool? VaryByDevice { get; set; }

    /// <summary>
    /// Gets or sets the 'vary by login' flag.
    /// </summary>
    [DataMember(Name = "varyByLogin")]
    [JsonPropertyName("varyByLogin")]
    public bool? VaryByLogin { get; set; }

    /// <summary>
    /// Gets or sets the 'vary by parameters' flag.
    /// </summary>
    [DataMember(Name = "varyByParameters")]
    [JsonPropertyName("varyByParameters")]
    public bool? VaryByParameters { get; set; }

    /// <summary>
    /// Gets or sets the 'vary by query string' flag.
    /// </summary>
    [DataMember(Name = "varyByQueryString")]
    [JsonPropertyName("varyByQueryString")]
    public bool? VaryByQueryString { get; set; }

    /// <summary>
    /// Gets or sets the 'vary by user' flag.
    /// </summary>
    [DataMember(Name = "varyByUser")]
    [JsonPropertyName("varyByUser")]
    public bool? VaryByUser { get; set; }

    /// <summary>
    /// Gets or sets the 'clear on index update' flag.
    /// </summary>
    [DataMember(Name = "clearOnIndexUpdate")]
    [JsonPropertyName("clearOnIndexUpdate")]
    public bool? ClearOnIndexUpdate { get; set; }
}