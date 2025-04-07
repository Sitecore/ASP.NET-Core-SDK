using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.Pages.Response;

/// <summary>
/// Class used to store the ClientData editing context, used to enable Pages functionality.
/// </summary>
public class ClientData
{
    /// <summary>
    /// Gets or sets the Canvas State data of the Editing Request.
    /// </summary>
    [JsonPropertyName("hrz-canvas-state")]
    public CanvasState? CanvasState { get; set; }

    /// <summary>
    /// Gets or sets the verification token used by the Pages canvas.
    /// </summary>
    [JsonPropertyName("hrz-canvas-verification-token")]
    public string? CanvasVerificationToken { get; set; }
}