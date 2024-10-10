using System.Text.Json;
using System.Text.Json.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

/// <summary>
/// Extension methods for <see cref="JsonSerializerOptions"/>.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds the default Layout Service serialization settings to the provided <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to add the default settings to.</param>
    /// <returns>The modified <see cref="JsonSerializerOptions"/> with the default settings added.</returns>
    public static JsonSerializerOptions AddLayoutServiceDefaults(this JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.PropertyNameCaseInsensitive = true;
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new FieldConverter());
        options.Converters.Add(new PlaceholderFeatureConverter(new PlaceholderParser(new FieldParser())));

        return options;
    }
}