namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;

/// <summary>
/// Represents the properties of a file in a Sitecore layout service response.
/// </summary>
public class File
{
    /// <summary>
    /// Gets or sets the source of the file.
    /// </summary>
    public string? Src { get; set; }

    /// <summary>
    /// Gets or sets the title of the file.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the file.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the display name of the file.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the keywords for the file.
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// Gets or sets the extension of the file.
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// Gets or sets the mime type of the file.
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Gets or sets the size of the file.
    /// </summary>
    public long? Size { get; set; }
}