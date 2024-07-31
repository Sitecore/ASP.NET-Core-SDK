using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <inheritdoc cref="IFieldParser"/>
public class FieldParser : IFieldParser
{
    /// <inheritdoc cref="IFieldParser.ParseFields"/>
    public Dictionary<string, IFieldReader> ParseFields(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        Dictionary<string, IFieldReader> fields = [];
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            string? key = reader.GetString();
            reader.Read();
            JsonDocument value = ParseField(ref reader);
            if (key != null)
            {
                fields.Add(key, new JsonSerializedField(value));
            }
        }

        return fields;
    }

    private static JsonDocument ParseField(ref Utf8JsonReader reader)
    {
        JsonDocument value = JsonDocument.ParseValue(ref reader);

        return value;
    }
}