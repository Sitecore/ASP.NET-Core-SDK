using System.Text.Json;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

/// <summary>
/// Extensions for <see cref="Utf8JsonReader"/>.
/// </summary>
public static class Utf8JsonReaderExtensions
{
    /// <summary>
    /// Determines if the reader has an object to read.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to use.</param>
    /// <param name="startDepth">The depth to ensure to match to determine finalizing the object read.</param>
    /// <returns>True if EndObject not reached and depths not matching.</returns>
    public static bool IsReadObjectAvailable(this ref Utf8JsonReader reader, int startDepth)
    {
        return reader.Read() &&
               (reader.TokenType != JsonTokenType.EndObject || reader.CurrentDepth != startDepth);
    }
}