namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Class used to define an items metadata information.
/// </summary>
public class MetaData
{
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///  Gets or sets the FieldId.
    /// </summary>
    public string FieldId { get; set; } = string.Empty;

    /// <summary>
    ///  Gets or sets the FieldType.
    /// </summary>
    public string FieldType { get; set; } = string.Empty;

    /// <summary>
    ///  Gets or sets the RawValue.
    /// </summary>
    public string RawValue { get; set; } = string.Empty;

    /// <summary>
    ///  Gets or sets the DataSource.
    /// </summary>
    public DataSource? DataSource { get; set; }
}