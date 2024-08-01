namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents the route information of a Sitecore layout service response.
/// </summary>
public class Route : FieldsReader
{
    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the device ID.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the route item ID.
    /// </summary>
    public string? ItemId { get; set; }

    /// <summary>
    /// Gets or sets the item language.
    /// </summary>
    public string? ItemLanguage { get; set; }

    /// <summary>
    /// Gets or sets the item version.
    /// </summary>
    public int? ItemVersion { get; set; }

    /// <summary>
    /// Gets or sets the layout ID.
    /// </summary>
    public string? LayoutId { get; set; }

    /// <summary>
    /// Gets or sets the template ID.
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the template name.
    /// </summary>
    public string? TemplateName { get; set; }

    /// <summary>
    /// Gets or sets the route name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the route display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets the route placeholders.
    /// </summary>
    public Dictionary<string, Placeholder> Placeholders { get; init; } = [];
}