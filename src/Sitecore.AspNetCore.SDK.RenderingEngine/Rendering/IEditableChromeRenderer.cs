using Microsoft.AspNetCore.Html;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// Supports rendering chrome data as HTML content that can be used to edit a web page.
/// </summary>
public interface IEditableChromeRenderer
{
    /// <summary>
    /// Generates the output HTML.
    /// </summary>
    /// <param name="chrome">The chrome data to render.</param>
    /// <returns>The HTML content to render.</returns>
    IHtmlContent Render(EditableChrome chrome);
}