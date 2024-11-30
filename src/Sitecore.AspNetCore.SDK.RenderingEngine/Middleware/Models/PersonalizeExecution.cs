namespace Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

/// <summary>
/// Personalize execution model.
/// </summary>
public class PersonalizeExecution
{
    /// <summary>
    /// Gets or sets the friendly id.
    /// </summary>
    public string FriendlyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variant ids.
    /// </summary>
    public List<string> VariantIds { get; set; } = [];
}