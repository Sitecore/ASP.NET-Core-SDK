using System.Text.Json;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;

/// <summary>
/// Layout Service GraphQL Response item data.
/// </summary>
public class ItemModel
{
    /// <summary>
    /// Gets or sets Rendered data.
    /// </summary>
    public JsonElement? Rendered { get; set; }
}