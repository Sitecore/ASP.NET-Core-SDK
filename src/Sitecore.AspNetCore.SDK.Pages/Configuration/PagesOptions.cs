﻿namespace Sitecore.AspNetCore.SDK.Pages.Configuration;

/// <summary>
/// The options to configure the Pages middleware.
/// </summary>
public class PagesOptions
{
    /// <summary>
    /// Key used to define the settings section in config.
    /// </summary>
    public static readonly string Key = "SitecoreXmcPages";

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
    /// Gets or sets the valid HTTP methods.
    /// </summary>
    public string ValidMethods { get; set; } = "GET, POST, OPTIONS, PUT, PATCH, DELETE";

    /// <summary>
    /// Gets or sets the valid headers.
    /// </summary>
    public string ValidHeaders { get; set; } = "Authorization";

    /// <summary>
    /// Gets or sets the Editing Secret.
    /// </summary>
    public string EditingSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of entries per page in a dictionary. The default value is set to 1000.
    /// </summary>
    public int DictionaryPageSize { get; set; } = 1000;
}