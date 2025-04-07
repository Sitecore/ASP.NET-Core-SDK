namespace Sitecore.AspNetCore.SDK.Pages.Models;

/// <summary>
/// The response object for the Pages Config endpoint.
/// </summary>
public class PagesConfigResponse
{
    /// <summary>
    /// Gets or sets the edit mode for the Pages Config endpoint.
    /// </summary>
    public List<string> Components { get; set; } = new();

    /// <summary>
    /// Gets or sets the edit mode for the Pages Config endpoint.
    /// </summary>
    public string? EditMode { get; set; }

    /// <summary>
    /// Gets or sets the edit mode for the Pages Config endpoint.
    /// </summary>
    public object Packages { get; set; } = new();
}
