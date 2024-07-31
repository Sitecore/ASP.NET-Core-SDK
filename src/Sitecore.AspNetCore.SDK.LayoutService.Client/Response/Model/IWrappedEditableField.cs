namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Exposes HTML markup values for an editable <see cref="IField"/> that has wrapping editable markup.
/// </summary>
public interface IWrappedEditableField : IField
{
    /// <summary>
    /// Gets or sets the HTML markup to render before this <see cref="IField"/> when editing.
    /// </summary>
    string EditableMarkupFirst { get; set; }

    /// <summary>
    /// Gets or sets the HTML markup to render after this <see cref="IField"/> when editing.
    /// </summary>
    string EditableMarkupLast { get; set; }
}