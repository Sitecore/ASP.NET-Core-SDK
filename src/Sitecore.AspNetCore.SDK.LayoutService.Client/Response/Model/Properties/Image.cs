namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;

/// <summary>
/// Represents the properties of an image in a Sitecore layout service response.
/// </summary>
public class Image
{
    /// <summary>
    /// Gets or sets the source of the image.
    /// </summary>
    public string? Src { get; set; }

    /// <summary>
    /// Gets or sets the alternative text of the image.
    /// </summary>
    public string? Alt { get; set; }

    /// <summary>
    /// Gets or sets the height of the image.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the width of the image.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the title of the image.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the number of whitespaces on the left and the right side of the image.
    /// </summary>
    public int? HSpace { get; set; }

    /// <summary>
    /// Gets or sets the number of whitespaces on the bottom and top side of the image.
    /// </summary>
    public int? VSpace { get; set; }

    /// <summary>
    /// Gets or sets the border thickness of the image.
    /// </summary>
    public int? Border { get; set; }

    /// <summary>
    /// Gets or sets the class of the image.
    /// </summary>
    public string? Class { get; set; }
}