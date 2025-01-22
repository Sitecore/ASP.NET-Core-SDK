using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents an arbitrary field in a Sitecore layout service response that contains a value that can be edited.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public class EditableField<TValue>
    : Field<TValue>, IEditableField
{
    /// <inheritdoc />
    [DataMember(Name = "editable")]
    [JsonPropertyName("editable")]
    public string EditableMarkup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the id of the Field.
    /// </summary>
    [DataMember(Name = "Id")]
    [JsonPropertyName("Id")]
    public string Id { get; set; } = string.Empty;

    /// <inheritdoc />
    public EditableChrome? OpeningChrome { get; set; }

    /// <inheritdoc />
    public EditableChrome? ClosingChrome { get; set; }

    /// <summary>
    /// Gets or Sets the MetaSata for the Field.
    /// </summary>
    public MetaData? MetaData { get; set; }
}