using System.Text.Json;
using System.Text.Json.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Constants;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <summary>
/// Handles conversion of a Placeholder feature collection.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlaceholderFeatureConverter"/> class.
/// </remarks>
/// <param name="fieldParser">The field parser.</param>
public class PlaceholderFeatureConverter(IFieldParser fieldParser)
    : JsonConverter<Placeholder>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeof(Placeholder).IsAssignableFrom(typeToConvert);

    /// <inheritdoc/>
    public override Placeholder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        Placeholder placeholder = [];

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            placeholder.Add(GetPlaceholderFeature(ref reader, typeToConvert, options));
        }

        return placeholder;
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

    private static Dictionary<string, string> GetParams(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int startDepth = reader.CurrentDepth;
        Dictionary<string, string> dictionary = [];

        while (IsReadObjectAvailable(ref reader, startDepth))
        {
            string? key = reader.GetString();

            reader.Read();

            string value = reader.GetString() ?? string.Empty;
            if (key != null)
            {
                dictionary.Add(key, value);
            }
        }

        return dictionary;
    }

    private static bool IsReadObjectAvailable(ref Utf8JsonReader reader, int startDepth)
    {
        return reader.Read() &&
               (reader.TokenType != JsonTokenType.EndObject || reader.CurrentDepth != startDepth);
    }

    private static EditableChrome GetEditableChromeInstance(IReadOnlyDictionary<string, object> properties)
    {
        EditableChrome editableChrome = new()
        {
            Name = GetPropertyByName<string>(properties, EditableChromePropertiesNames.Name) ?? string.Empty,
            Attributes = GetPropertyByName<Dictionary<string, string>>(properties, EditableChromePropertiesNames.Attributes) ?? [],
            Content = GetPropertyByName<string>(properties, EditableChromePropertiesNames.Contents) ?? string.Empty,
            Type = GetPropertyByName<string>(properties, EditableChromePropertiesNames.Type) ?? string.Empty
        };
        return editableChrome;
    }

    private static Component GetComponentInstance(Dictionary<string, object> properties)
    {
        Component component = new()
        {
            Id = GetPropertyByName<string>(properties, ComponentPropertiesNames.Id) ?? string.Empty,
            Fields = GetPropertyByName<Dictionary<string, IFieldReader>>(properties, ComponentPropertiesNames.Fields) ?? [],
            Name = GetPropertyByName<string>(properties, ComponentPropertiesNames.ComponentName) ?? string.Empty,
            Parameters = GetPropertyByName<Dictionary<string, string>>(properties, ComponentPropertiesNames.Params) ?? [],
            Placeholders = GetPropertyByName<Dictionary<string, Placeholder>>(properties, ComponentPropertiesNames.Placeholders) ?? [],
            DataSource = GetPropertyByName<string>(properties, ComponentPropertiesNames.DataSource) ?? string.Empty
        };

        return component;
    }

    private static TProperty? GetPropertyByName<TProperty>(IReadOnlyDictionary<string, object> properties, string property)
        where TProperty : class
    {
        if (properties.TryGetValue(property, out object? obj))
        {
            return (TProperty)obj;
        }

        return null;
    }

    private IPlaceholderFeature GetPlaceholderFeature(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int startDepth = reader.CurrentDepth;
        Dictionary<string, object> properties = [];

        while (IsReadObjectAvailable(ref reader, startDepth))
        {
            string? propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case ComponentPropertiesNames.Fields:
                    properties.TryAdd(propertyName, fieldParser.ParseFields(ref reader));
                    break;
                case ComponentPropertiesNames.Placeholders:
                    properties.TryAdd(propertyName, GetPlaceholders(ref reader, typeToConvert, options));
                    break;
                case ComponentPropertiesNames.Params:
                case EditableChromePropertiesNames.Attributes:
                    properties.TryAdd(propertyName, GetParams(ref reader));
                    break;
                case ComponentPropertiesNames.Id:
                case ComponentPropertiesNames.DataSource:
                case ComponentPropertiesNames.ComponentName:
                case EditableChromePropertiesNames.Name:
                case EditableChromePropertiesNames.Contents:
                case EditableChromePropertiesNames.Type:
                    properties.TryAdd(propertyName, reader.GetString() ?? string.Empty);
                    break;
            }
        }

        if (properties.TryGetValue(LayoutServiceClientConstants.SitecoreChromes.ChromeTypeName, out object? type) &&
            type.ToString() == LayoutServiceClientConstants.SitecoreChromes.ChromeTypeValue)
        {
            return GetEditableChromeInstance(properties);
        }

        return GetComponentInstance(properties);
    }

    private Dictionary<string, Placeholder> GetPlaceholders(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int startDepth = reader.CurrentDepth;
        Dictionary<string, Placeholder> placeHolderDictionary = [];

        while (IsReadObjectAvailable(ref reader, startDepth))
        {
            string? key = reader.GetString();
            reader.Read();
            Placeholder placeHolder = Read(ref reader, typeToConvert, options);
            if (key != null)
            {
                placeHolderDictionary.Add(key, placeHolder);
            }
        }

        return placeHolderDictionary;
    }
}