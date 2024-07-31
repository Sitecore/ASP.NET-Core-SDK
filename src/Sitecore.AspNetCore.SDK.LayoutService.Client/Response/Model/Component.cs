using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents component information for a Sitecore layout service response.
/// </summary>
public class Component : FieldsReader, IPlaceholderFeature
{
    /// <summary>
    /// Gets or sets the ID of the component.
    /// </summary>
    [DataMember(Name = "uid")]
    [JsonPropertyName("uid")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the name of the component.
    /// </summary>
    [DataMember(Name = "componentName")]
    [JsonPropertyName("componentName")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the datasource of the component.
    /// </summary>
    public string DataSource { get; set; } = "available-in-connected-mode";

    /// <summary>
    /// Gets the parameters for the component.
    /// </summary>
    [DataMember(Name = "params")]
    [JsonPropertyName("params")]
    public Dictionary<string, string> Parameters { get; init; } = [];

    /// <summary>
    /// Gets the placeholders for the component.
    /// </summary>
    public Dictionary<string, Placeholder> Placeholders { get; init; } = [];
}