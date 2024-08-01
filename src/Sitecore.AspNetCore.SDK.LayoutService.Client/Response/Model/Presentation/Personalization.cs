using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;

/// <summary>
/// Represents the personalization information for a device rendering returned in a Sitecore layout service response.
/// </summary>
public class Personalization
{
    /// <summary>
    /// Gets or sets the rules.
    /// </summary>
    [DataMember(Name = "rules")]
    [JsonPropertyName("rules")]
    public string? Rules { get; set; }

    /// <summary>
    /// Gets or sets the conditions.
    /// </summary>
    [DataMember(Name = "conditions")]
    [JsonPropertyName("conditions")]
    public string? Conditions { get; set; }

    /// <summary>
    /// Gets or sets the multivariate test ID.
    /// </summary>
    [DataMember(Name = "multiVariateTestId")]
    [JsonPropertyName("multiVariateTestId")]
    public string? MultiVariateTestId { get; set; }

    /// <summary>
    /// Gets or sets the personalization test.
    /// </summary>
    [DataMember(Name = "personalizationTest")]
    [JsonPropertyName("personalizationTest")]
    public string? PersonalizationTest { get; set; }
}