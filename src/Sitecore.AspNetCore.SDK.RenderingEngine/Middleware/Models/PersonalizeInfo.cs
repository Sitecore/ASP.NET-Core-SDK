namespace Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

public class PersonalizeInfo
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    public int? Version { get; set; }

    /// <summary>
    /// Gets or sets the variant ids.
    /// </summary>
    public string?[] VariantIds { get; set; }
}