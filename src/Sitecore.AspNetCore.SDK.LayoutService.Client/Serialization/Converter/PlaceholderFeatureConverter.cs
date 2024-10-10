using System.Text.Json;
using System.Text.Json.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <summary>
/// Handles conversion of a Placeholder feature collection.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlaceholderFeatureConverter"/> class.
/// </remarks>
/// <param name="placeholderParser">The placeholder parser.</param>
public class PlaceholderFeatureConverter(IPlaceholderParser placeholderParser)
    : JsonConverter<Placeholder>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeof(Placeholder).IsAssignableFrom(typeToConvert);

    /// <inheritdoc/>
    public override Placeholder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        return placeholderParser.ParsePlaceholder(ref reader, options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Placeholder value, JsonSerializerOptions options)
    {
        // NOTE We do not need a null check for "value" since this Converter won't handle "null"
        ArgumentNullException.ThrowIfNull(options);

        writer.WriteStartArray();
        foreach (IPlaceholderFeature feature in value)
        {
            JsonSerializer.Serialize(writer, feature, feature.GetType(), options);
        }

        writer.WriteEndArray();
    }
}