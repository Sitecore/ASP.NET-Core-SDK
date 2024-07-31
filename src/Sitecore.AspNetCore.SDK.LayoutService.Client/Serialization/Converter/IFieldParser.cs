using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

/// <summary>
/// Handles conversion of a Field.
/// </summary>
public interface IFieldParser
{
    /// <summary>
    /// Reads Json and converts to <see cref="IFieldReader"/>.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>Parsed fields.</returns>
    Dictionary<string, IFieldReader> ParseFields(ref Utf8JsonReader reader);
}