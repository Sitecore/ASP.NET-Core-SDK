using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;

/// <inheritdoc/>
/// <summary>
/// Initializes a new instance of the <see cref="JsonLayoutServiceSerializer"/> class.
/// </summary>
public class JsonLayoutServiceSerializer : ISitecoreLayoutSerializer
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions =
        new JsonSerializerOptions().AddLayoutServiceDefaults();

    private readonly JsonSerializerOptions? _serializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLayoutServiceSerializer"/> class.
    /// </summary>
    public JsonLayoutServiceSerializer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLayoutServiceSerializer"/> class.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use.</param>
    public JsonLayoutServiceSerializer(JsonSerializerOptions options)
    {
        _serializerOptions = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Gets the default <see cref="JsonSerializerOptions"/> used.
    /// </summary>
    /// <returns>The default <see cref="JsonSerializerOptions"/> instance.</returns>
    public static JsonSerializerOptions GetDefaultSerializerOptions()
    {
        return DefaultSerializerOptions;
    }

    /// <inheritdoc/>
    public SitecoreLayoutResponseContent? Deserialize(string data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        SitecoreLayoutResponseContent? layoutResponseContent = JsonSerializer.Deserialize<SitecoreLayoutResponseContent>(data, _serializerOptions ?? DefaultSerializerOptions);

        Context? scContext = layoutResponseContent?.Sitecore?.Context;
        if (scContext != null && layoutResponseContent != null)
        {
            JsonDocument doc = JsonDocument.Parse(data);
            layoutResponseContent.ContextRawData = doc.RootElement
                .GetProperty(LayoutServiceClientConstants.Serialization.SitecoreDataPropertyName)
                .GetProperty(LayoutServiceClientConstants.Serialization.ContextPropertyName)
                .GetRawText();
        }

        return layoutResponseContent;
    }
}