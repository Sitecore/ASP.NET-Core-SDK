using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// An <see cref="IComponentRenderer"/> that will render Partial Views.
/// </summary>
public class PartialViewComponentRenderer : IComponentRenderer
{
    private readonly string _locator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialViewComponentRenderer"/> class.
    /// </summary>
    /// <param name="locator">The string to use when locating the Partial View.</param>
    public PartialViewComponentRenderer(string locator)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locator);
        _locator = locator;
    }

    /// <summary>
    /// Creates an instance of a <see cref="ComponentRendererDescriptor"/> for the <see cref="PartialViewComponentRenderer"/> class.
    /// </summary>
    /// <param name="match">A predicate to use when attempting to match a layout component.</param>
    /// <param name="locator">The string to use when locating the Partial View.</param>
    /// <returns>An instance of <see cref="ComponentRendererDescriptor"/> that describes the <see cref="PartialViewComponentRenderer"/>.</returns>
    public static ComponentRendererDescriptor Describe(Predicate<string> match, string locator)
    {
        ArgumentNullException.ThrowIfNull(match);
        ArgumentException.ThrowIfNullOrWhiteSpace(locator);
        return new ComponentRendererDescriptor(
            match,
            sp => ActivatorUtilities.CreateInstance<PartialViewComponentRenderer>(sp, locator),
            locator);
    }

    /// <inheritdoc />
    public Task<IHtmlContent> Render(ISitecoreRenderingContext renderingContext, ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(renderingContext);
        ArgumentNullException.ThrowIfNull(viewContext);
        IHtmlHelper htmlHelper = renderingContext.RenderingHelpers?.HtmlHelper ?? throw new NullReferenceException();
        ((IViewContextAware)htmlHelper).Contextualize(viewContext);

        return htmlHelper.PartialAsync(_locator, renderingContext.Component);
    }
}