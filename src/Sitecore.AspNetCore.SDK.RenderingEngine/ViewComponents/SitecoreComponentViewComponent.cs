using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.ViewComponents;

/// <summary>
/// Default view with model binding to render components.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SitecoreComponentViewComponent"/> class.
/// </remarks>
/// <param name="binder">The <see cref="IViewModelBinder"/> to use when binding the model to the view component.</param>
[ViewComponent]
public class SitecoreComponentViewComponent(IViewModelBinder binder)
    : BindingViewComponent(binder)
{
    /// <summary>
    /// Executes the view component.
    /// </summary>
    /// <param name="modelType">The type of the model to use when generating the view component output.</param>
    /// <param name="viewName">The name of the view to use when generating the view component output.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<IViewComponentResult> InvokeAsync(Type modelType, string viewName)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewName);
        return await BindView(modelType, viewName).ConfigureAwait(false);
    }
}