namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Exposes HTML markup values for an editable <see cref="IField"/>.
/// </summary>
public interface IEditableField : IField
{
    /// <summary>
    /// Gets or sets the HTML markup for this <see cref="IField"/> when editing.
    /// </summary>
    public string EditableMarkup { get; set; }
}