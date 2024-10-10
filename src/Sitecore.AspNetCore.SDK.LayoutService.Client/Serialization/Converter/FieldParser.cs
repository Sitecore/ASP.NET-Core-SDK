using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <inheritdoc cref="IFieldParser"/>
public class FieldParser : IFieldParser
{
    /// <summary>
    /// Field key for custom content created by Custom Content Resolvers.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global - Must be accessible for people using the SDK.
    public const string CustomContentFieldKey = "CustomContent";

    /// <inheritdoc />
    public Dictionary<string, IFieldReader> ParseFields(ref Utf8JsonReader reader)
    {
        Dictionary<string, IFieldReader> result = [];
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                result = ParseStandardFields(ref reader);
                break;
            default:
                result.Add(CustomContentFieldKey, ParseField(ref reader));
                break;
        }

        return result;
    }

    private static Dictionary<string, IFieldReader> ParseStandardFields(ref Utf8JsonReader reader)
    {
        int startDepth = reader.CurrentDepth;
        Dictionary<string, IFieldReader> result = [];
        while (reader.IsReadObjectAvailable(startDepth))
        {
            string? key = reader.GetString();
            reader.Read();
            if (key != null)
            {
                result.Add(key, ParseField(ref reader));
            }
        }

        return result;
    }

    private static JsonSerializedField ParseField(ref Utf8JsonReader reader)
    {
        JsonDocument value = JsonDocument.ParseValue(ref reader);
        return new JsonSerializedField(value);
    }
}