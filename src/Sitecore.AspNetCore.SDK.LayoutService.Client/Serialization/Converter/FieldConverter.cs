using System.Text.Json;
using System.Text.Json.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <summary>
/// Handles conversion of a Fields.
/// </summary>
public class FieldConverter : JsonConverter<IFieldReader>
{
    /// <inheritdoc cref="JsonConverter.CanConvert"/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(IFieldReader);

    /// <inheritdoc/>
    public override IFieldReader Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        JsonDocument doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.ValueKind switch
        {
            JsonValueKind.Object or JsonValueKind.Array => new JsonSerializedField(doc),
            _ => throw new JsonException($"Expected an array or object when deserializing a {typeof(IFieldReader)}. Found {reader.TokenType}"),
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IFieldReader value, JsonSerializerOptions options)
    {
        // NOTE We do not need a null check for "value" since this Converter won't handle "null"
        ArgumentNullException.ThrowIfNull(options);
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}