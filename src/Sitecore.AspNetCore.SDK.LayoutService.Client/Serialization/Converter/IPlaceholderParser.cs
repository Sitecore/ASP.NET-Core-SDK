using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <summary>
/// Handles conversion of <see cref="Placeholder"/>.
/// </summary>
public interface IPlaceholderParser
{
    /// <summary>
    /// Parse a single <see cref="Placeholder"/> from the <see cref="Utf8JsonReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/>.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/>.</param>
    /// <returns>A <see cref="Placeholder"/>.</returns>
    Placeholder ParsePlaceholder(ref Utf8JsonReader reader, JsonSerializerOptions options);
}