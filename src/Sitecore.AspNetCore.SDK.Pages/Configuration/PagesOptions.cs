namespace Sitecore.AspNetCore.SDK.Pages.Configuration;

/// <summary>
/// The options to configure the Pages middleware.
/// </summary>
public class PagesOptions
{
    /// <summary>
    /// Gets or sets the config endpoint for Pages MetaData mode.
    /// </summary>
    public string ConfigEndpoint { get; set; } = "/api/editing/config";

    /// <summary>
    /// Gets or sets the render endpoint for Pages MetaData mode.
    /// </summary>
    public string RenderEndpoint { get; set; } = "/api/editing/render";

    /// <summary>
    /// Gets or sets the valid editing origin for all editing requests.
    /// </summary>
    public string ValidEditingOrigin { get; set; } = "https://pages.sitecorecloud.io";

    /// <summary>
    /// Gets or sets the valid origins for the head to run under.
    /// </summary>
    public string ValidOrigins { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Editing Secret.
    /// </summary>
    public string EditingSecret { get; set; } = string.Empty;
}