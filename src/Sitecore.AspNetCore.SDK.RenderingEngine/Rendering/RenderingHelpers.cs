using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// Rendering helpers.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RenderingHelpers"/> class.
/// </remarks>
/// <param name="viewComponentHelper">The IViewComponentHelper instance <see cref="IViewComponentHelper"/>.</param>
/// <param name="htmlHelper">The IHtmlHelper instance <see cref="IHtmlHelper"/>.</param>
public class RenderingHelpers(IViewComponentHelper viewComponentHelper, IHtmlHelper htmlHelper)
{
    /// <summary>
    /// Gets IViewComponentHelper instance.
    /// </summary>
    public IViewComponentHelper ViewComponentHelper { get; private set; } = viewComponentHelper;

    /// <summary>
    /// Gets IHtmlHelper instance.
    /// </summary>
    public IHtmlHelper HtmlHelper { get; private set; } = htmlHelper;
}