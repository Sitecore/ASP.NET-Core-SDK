using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// An <see cref="IComponentRenderer"/> that will render View Components that are bound to a specified Model type.
/// </summary>
/// <typeparam name="TModel">The model type to bind to the View Component.</typeparam>
public class ModelBoundViewComponentComponentRenderer<TModel> : IComponentRenderer
{
    private readonly string _locator;
    private readonly string _viewName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelBoundViewComponentComponentRenderer{TModel}"/> class.
    /// </summary>
    /// <param name="locator">The string to use when locating the View Component.</param>
    /// <param name="viewName">The name of the view to use when rendering.</param>
    public ModelBoundViewComponentComponentRenderer(string locator, string viewName)
    {
        ArgumentNullException.ThrowIfNull(locator);
        ArgumentNullException.ThrowIfNull(viewName);

        _locator = locator;
        _viewName = viewName;
    }

    /// <inheritdoc />
    public Task<IHtmlContent> Render(ISitecoreRenderingContext renderingContext, ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(renderingContext);
        ArgumentNullException.ThrowIfNull(viewContext);
        IViewComponentHelper viewComponentHelper = renderingContext.RenderingHelpers?.ViewComponentHelper ?? throw new NullReferenceException();
        ((IViewContextAware)viewComponentHelper).Contextualize(viewContext);

        return viewComponentHelper.InvokeAsync(_locator, new { modelType = typeof(TModel), viewName = _viewName });
    }
}