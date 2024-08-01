using System.Text.Json;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;

/// <inheritdoc />
internal class ChromeDataSerializer : IChromeDataSerializer
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <inheritdoc />
    public string Serialize(Dictionary<string, object?> chromeData)
    {
        return JsonSerializer.Serialize(chromeData, DefaultSerializerOptions);
    }
}