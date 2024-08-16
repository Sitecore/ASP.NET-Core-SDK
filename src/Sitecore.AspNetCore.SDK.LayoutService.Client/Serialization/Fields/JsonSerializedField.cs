using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;

/// <summary>
/// Encapsulates a <see cref="JsonDocument"/> for later deserialization as a <see cref="Field"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JsonSerializedField"/> class.
/// </remarks>
/// <param name="doc">The instance of <see cref="JsonDocument"/> for later deserialization.</param>
public class JsonSerializedField(JsonDocument doc)
    : FieldReader
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions().AddLayoutServiceDefaults();

    private readonly string _json = doc != null ? doc.RootElement.GetRawText() : throw new ArgumentNullException(nameof(doc));

    /// <inheritdoc/>
    public override string ToString()
    {
        return _json;
    }

    /// <inheritdoc/>
    protected override object? HandleRead(Type type)
    {
        // NOTE The JsonSerializerOptions used to be delivered through the deserialization but are now locked here inside the class because
        // the caches inside appear to be incompatible if the options instance was used for different deserialization outside the library before.
        // We should test when .NET8+ releases whether this is fixed and whether we can use the options from the deserialization again.
        return JsonSerializer.Deserialize(_json, type, Options);
    }
}