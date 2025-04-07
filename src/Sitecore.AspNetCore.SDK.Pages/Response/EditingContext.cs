using System.Text.Json.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.Pages.Response;

/// <summary>
/// Class used to extend the standard Sitecore Context with editing specific context data.
/// </summary>
public class EditingContext : Context
{
    /// <summary>
    /// Gets or sets the Client data property used to hold the editing specific client data.
    /// </summary>
    [JsonPropertyName("clientData")]
    public ClientData? ClientData { get; set; }

    /// <summary>
    /// Gets or sets the ClientScripts property used to store the scripts that need to be included in the client app for Pages to function.
    /// </summary>
    [JsonPropertyName("clientScripts")]
    public string[]? ClientScripts { get; set; }
}