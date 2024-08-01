namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

/// <summary>
/// Defines the keys that may be included in the Sitecore layout service request.
/// </summary>
public static class RequestKeys
{
    /// <summary>
    /// The key name for request site name.
    /// </summary>
    public const string SiteName = "sc_site";

    /// <summary>
    /// The key name for request path.
    /// </summary>
    public const string Path = "item";

    /// <summary>
    /// The key name for request language.
    /// </summary>
    public const string Language = "sc_lang";

    /// <summary>
    /// The key name for request API key.
    /// </summary>
    public const string ApiKey = "sc_apikey";

    /// <summary>
    /// The key name for request mode.
    /// </summary>
    public const string Mode = "sc_mode";

    /// <summary>
    /// The key name for device ID.
    /// </summary>
    public const string Device = "sc_device";

    /// <summary>
    /// The key name for request item ID.
    /// </summary>
    public const string ItemId = "sc_itemid";

    /// <summary>
    /// The key name for request authentication header.
    /// </summary>
    public const string AuthHeaderKey = "sc_auth_header_key";

    /// <summary>
    /// The key name for request preview date.
    /// </summary>
    public const string PreviewDate = "sc_date";
}