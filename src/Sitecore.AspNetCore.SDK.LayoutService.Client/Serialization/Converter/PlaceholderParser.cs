using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Constants;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <inheritdoc />
public class PlaceholderParser(IFieldParser fieldParser) : IPlaceholderParser
{
    /// <inheritdoc />
    public Placeholder ParsePlaceholder(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        Placeholder placeholder = [];

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            placeholder.Add(GetPlaceholderFeature(ref reader, options));
        }

        return placeholder;
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
            DataSource = GetPropertyByName<string>(properties, ComponentPropertiesNames.DataSource) ?? string.Empty,
            Experiences = GetPropertyByName<Dictionary<string, Component>>(properties, ComponentPropertiesNames.Experiences) ?? []
        };

        return component;
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

    private static TProperty? GetPropertyByName<TProperty>(IReadOnlyDictionary<string, object> properties, string property)
        where TProperty : class
    {
        if (properties.TryGetValue(property, out object? obj))
        {
            return (TProperty)obj;
        }

        return null;
    }

    private IPlaceholderFeature GetPlaceholderFeature(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int startDepth = reader.CurrentDepth;
        Dictionary<string, object> properties = [];

        while (reader.IsReadObjectAvailable(startDepth))
        {
            string? propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case ComponentPropertiesNames.Fields:
                    properties.TryAdd(propertyName, fieldParser.ParseFields(ref reader));
                    break;
                case ComponentPropertiesNames.Placeholders:
                    properties.TryAdd(propertyName, ParsePlaceholders(ref reader, options));
                    break;
                case ComponentPropertiesNames.Params:
                case EditableChromePropertiesNames.Attributes:
                    properties.TryAdd(propertyName, JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options) ?? []);
                    break;
                case ComponentPropertiesNames.Experiences:
                    properties.TryAdd(propertyName, JsonSerializer.Deserialize<Dictionary<string, Component>>(ref reader, options) ?? []);
                    break;
                case ComponentPropertiesNames.Id:
                case ComponentPropertiesNames.DataSource:
                case ComponentPropertiesNames.ComponentName:
                case EditableChromePropertiesNames.Name:
                case EditableChromePropertiesNames.Contents:
                case EditableChromePropertiesNames.Type:
                    properties.TryAdd(propertyName, reader.GetString() ?? string.Empty);
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(propertyName))
                    {
                        reader.Skip();
                    }
                    else
                    {
                        properties.TryAdd(propertyName, JsonDocument.ParseValue(ref reader));
                    }

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

    private Dictionary<string, Placeholder> ParsePlaceholders(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int startDepth = reader.CurrentDepth;
        Dictionary<string, Placeholder> placeHolderDictionary = [];

        while (reader.IsReadObjectAvailable(startDepth))
        {
            string? key = reader.GetString();
            reader.Read();
            Placeholder placeHolder = ParsePlaceholder(ref reader, options);
            if (key != null)
            {
                placeHolderDictionary.Add(key, placeHolder);
            }
        }

        return placeHolderDictionary;
    }
}