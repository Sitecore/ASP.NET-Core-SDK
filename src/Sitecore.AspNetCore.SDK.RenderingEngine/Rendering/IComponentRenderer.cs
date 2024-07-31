using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// Supports rendering HTML content that can be used in a web page.
/// </summary>
public interface IComponentRenderer
{
    /// <summary>
    /// Generates the output HTML.
    /// </summary>
    /// <param name="renderingContext">The current <see cref="ISitecoreRenderingContext"/>.</param>
    /// <param name="viewContext">The current <see cref="ViewContext"/>.</param>
    /// <returns>The HTML content to render.</returns>
    Task<IHtmlContent> Render(ISitecoreRenderingContext renderingContext, ViewContext viewContext);
}