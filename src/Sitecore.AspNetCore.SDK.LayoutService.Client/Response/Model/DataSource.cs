using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Class used to define an items datasource information.
/// </summary>
public class DataSource
{
    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Language.
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Revision.
    /// </summary>
    public string Revision { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Version.
    /// </summary>
    public int Version { get; set; }
}