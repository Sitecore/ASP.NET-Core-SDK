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

    /// <summary>
    /// Gets or sets the EditableChrome used to render the opening chrome for this field.
    /// </summary>
    public EditableChrome? OpeningChrome { get; set; }

    /// <summary>
    /// Gets or sets the EditableChrome used to render the closing chrome for this field.
    /// </summary>

    public EditableChrome? ClosingChrome { get; set; }
}