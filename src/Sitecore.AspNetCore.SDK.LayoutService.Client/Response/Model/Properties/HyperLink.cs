using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;

/// <summary>
/// Represents the properties of a hyperlink in a Sitecore layout service response.
/// </summary>
public class HyperLink
{
    /// <summary>
    /// Gets or sets the href of the hyperlink.
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets the text of the hyperlink.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the target of the hyperlink.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets the CSS class of the hyperlink.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the title of the hyperlink.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the anchor of the hyperlink.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Anchor { get; set; }
}