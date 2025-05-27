using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.Pages.Response;

/// <summary>
/// Class used to store the ClientData editing context, used to enable Pages functionality.
/// </summary>
public class CanvasState
{
    /// <summary>
    /// Gets or sets the id of the item being edited.
    /// </summary>
    [JsonPropertyName("itemId")]
    public string? ItemId { get; set; }

    /// <summary>
    /// Gets or sets the Version of the item being edited.
    /// </summary>
    [JsonPropertyName("itemVersion")]
    public int? ItemVersion { get; set; }

    /// <summary>
    /// Gets or sets the name of the site being edited.
    /// </summary>
    [JsonPropertyName("siteName")]
    public string? SiteName { get; set; }

    /// <summary>
    /// Gets or sets the language of the item being edited.
    /// </summary>
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the id of the device being edited.
    /// </summary>
    [JsonPropertyName("deviceId")]
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the current page mode.
    /// </summary>
    [JsonPropertyName("pageMode")]
    public string? PageMode { get; set; }

    /// <summary>
    /// Gets or sets the current id of the variant being edited.
    /// </summary>
    [JsonPropertyName("variant")]
    public string? Variant { get; set; }
}