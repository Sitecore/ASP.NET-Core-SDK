using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// An <see cref="IComponentRenderer"/> that will render View Components.
/// </summary>
public class ViewComponentComponentRenderer : IComponentRenderer
{
    private readonly string _locator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewComponentComponentRenderer"/> class.
    /// </summary>
    /// <param name="locator">The string to use when locating the View Component.</param>
    public ViewComponentComponentRenderer(string locator)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locator);
        _locator = locator;
    }

    /// <summary>
    /// Creates an instance of a <see cref="ComponentRendererDescriptor"/> for the <see cref="ViewComponentComponentRenderer"/> class.
    /// </summary>
    /// <param name="match">A predicate to use when attempting to match a layout component.</param>
    /// <param name="locator">The string to use when locating the View Component.</param>
    /// <param name="componentName">The string to use describe the name of the components.</param>
    /// <returns>An instance of <see cref="ComponentRendererDescriptor"/>.</returns>
    public static ComponentRendererDescriptor Describe(Predicate<string> match, string locator, string componentName)
    {
        ArgumentNullException.ThrowIfNull(match);
        ArgumentException.ThrowIfNullOrWhiteSpace(locator);
        ArgumentException.ThrowIfNullOrWhiteSpace(componentName);

        return new ComponentRendererDescriptor(
            match,
            sp => ActivatorUtilities.CreateInstance<ViewComponentComponentRenderer>(sp, locator),
            componentName);
    }

    /// <inheritdoc />
    public Task<IHtmlContent> Render(ISitecoreRenderingContext renderingContext, ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(renderingContext);
        ArgumentNullException.ThrowIfNull(viewContext);

        IViewComponentHelper viewComponentHelper = renderingContext.RenderingHelpers?.ViewComponentHelper ?? throw new NullReferenceException();
        ((IViewContextAware)viewComponentHelper).Contextualize(viewContext);

        return viewComponentHelper.InvokeAsync(_locator);
    }
}