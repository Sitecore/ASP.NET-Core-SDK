using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents an arbitrary field in a Sitecore layout service response
/// that contains a value that can be edited using wrapped HTML markup.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public class WrappedEditableField<TValue> : EditableField<TValue>, IWrappedEditableField
{
    /// <inheritdoc />
    [DataMember(Name = "editableFirstPart")]
    [JsonPropertyName("editableFirstPart")]
    public string EditableMarkupFirst { get; set; } = string.Empty;

    /// <inheritdoc />
    [DataMember(Name = "editableLastPart")]
    [JsonPropertyName("editableLastPart")]
    public string EditableMarkupLast { get; set; } = string.Empty;
}