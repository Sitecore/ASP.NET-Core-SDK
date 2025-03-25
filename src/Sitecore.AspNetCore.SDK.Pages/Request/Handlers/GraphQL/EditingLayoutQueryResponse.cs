using System.Text.Json.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;

namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <summary>
/// Layout Service GraphQL Response.
/// </summary>
public class EditingLayoutQueryResponse
{
    /// <summary>
    /// Gets or sets Item for the Editing Layout Response.
    /// </summary>
    public ItemModel? Item { get; set; }

    /// <summary>
    /// Gets or sets the Site for the Editing Layout Response.
    /// </summary>
    public Site Site { get; set; } = new();
}