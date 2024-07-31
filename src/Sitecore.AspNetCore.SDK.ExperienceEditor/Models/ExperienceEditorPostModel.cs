namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Models;

/// <summary>
/// Represents the model used to store the experience editor post.
/// </summary>
public class ExperienceEditorPostModel
{
    /// <summary>
    /// Gets or sets the Id which has the name of the JSS app in the content tree/configuration.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the function name.
    /// </summary>
    public string? FunctionName { get; set; }

    /// <summary>
    /// Gets or sets the module name.
    /// </summary>
    public string? ModuleName { get; set; }

    /// <summary>
    /// Gets or sets the Args which is an array that contains JSON strings.
    /// </summary>
    /// <remarks>
    /// By default, the array has the following structure:
    /// <code>
    /// {
    ///     request path,
    ///     serialized data {
    ///         sitecore (a root property) {
    ///             context (contains additional details, like language, site, user and the item path),
    ///             route (contains the item's properties and layout details) } },
    ///     serialized view bag {
    ///         language,
    ///         dictionary (localization),
    ///         httpContext {
    ///             request {
    ///                 url,
    ///                 path,
    ///                 querystring (a key-value dictionary ),
    ///                 userAgent } } } }
    /// </code>
    /// The item path is the path that would be seen, when navigating to it on frontend, i.e. it is a site relative link.
    /// </remarks>
    public List<string> Args { get; set; } = [];

    /// <summary>
    /// Gets or sets the Jss Editing Secret.
    /// </summary>
    public string? JssEditingSecret { get; set; }
}