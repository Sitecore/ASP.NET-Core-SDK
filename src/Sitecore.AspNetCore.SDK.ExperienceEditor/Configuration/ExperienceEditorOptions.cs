using Microsoft.AspNetCore.Http;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;

/// <summary>
/// The options to configure the experience editor middleware.
/// </summary>
public class ExperienceEditorOptions
{
    /// <summary>
    /// Gets or sets the endpoint that represent editing application URLs.
    /// </summary>
    public string Endpoint { get; set; } = "/jss-render";

    /// <summary>
    /// Gets or sets the action list to configure the <see cref="HttpRequest"/> handler for Experience Editor custom post requests.
    /// </summary>
    public ICollection<Action<SitecoreLayoutResponseContent, string, HttpRequest>> ItemMappings { get; set; } = [];

    /// <summary>
    /// Gets or sets the Jss Editing Secret.
    /// </summary>
    public string JssEditingSecret { get; set; } = string.Empty;
}