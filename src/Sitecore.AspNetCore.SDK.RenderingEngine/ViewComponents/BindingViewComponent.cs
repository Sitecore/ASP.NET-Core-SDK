using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.ViewComponents;

/// <summary>
/// Provides delayed binding for a <see cref="ViewComponent"/>.
/// </summary>
public abstract class BindingViewComponent : ViewComponent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindingViewComponent"/> class.
    /// </summary>
    /// <param name="binder">The <see cref="IViewModelBinder"/> to enable binding.</param>
    protected BindingViewComponent(IViewModelBinder binder)
    {
        ArgumentNullException.ThrowIfNull(binder);
        Binder = binder;
    }

    /// <summary>
    /// Gets the instance of <see cref="IViewModelBinder"/> used for binding.
    /// </summary>
    protected IViewModelBinder Binder { get; }

    /// <summary>
    /// Returns the default view using a bound model of <typeparamref name="TModel"/>.
    /// </summary>
    /// <typeparam name="TModel">The model to be bound.</typeparam>
    /// <returns>The <see cref="IViewComponentResult"/>.</returns>
    public virtual async Task<IViewComponentResult> BindView<TModel>()
        where TModel : class, new()
    {
        TModel model = await Binder.Bind<TModel>(ViewContext).ConfigureAwait(false);
        return View(model);
    }

    /// <summary>
    /// Returns the default view bound to the given model.
    /// </summary>
    /// <typeparam name="TModel">The model to be bound.</typeparam>
    /// <param name="model">An instance of <typeparamref name="TModel"/> to be bound.</param>
    /// <returns>The <see cref="IViewComponentResult"/>.</returns>
    public virtual async Task<IViewComponentResult> BindView<TModel>(TModel model)
        where TModel : class
    {
        await Binder.Bind(model, ViewContext).ConfigureAwait(false);
        return View(model);
    }

    /// <summary>
    /// Returns the specified view using a bound model of <typeparamref name="TModel"/>.
    /// </summary>
    /// <typeparam name="TModel">The model to be bound.</typeparam>
    /// <param name="viewName">The view to be returned.</param>
    /// <returns>The <see cref="IViewComponentResult"/>.</returns>
    public virtual async Task<IViewComponentResult> BindView<TModel>(string viewName)
        where TModel : class, new()
    {
        TModel model = await Binder.Bind<TModel>(ViewContext).ConfigureAwait(false);
        return View(viewName, model);
    }

    /// <summary>
    /// Returns the specified view bound to the given model.
    /// </summary>
    /// <typeparam name="TModel">The model to be bound.</typeparam>
    /// <param name="viewName">The view to be returned.</param>
    /// <param name="model">An instance of <typeparamref name="TModel"/> to be bound.</param>
    /// <returns>The <see cref="IViewComponentResult"/>.</returns>
    public virtual async Task<IViewComponentResult> BindView<TModel>(string viewName, TModel model)
        where TModel : class
    {
        await Binder.Bind(model, ViewContext).ConfigureAwait(false);
        return View(viewName, model);
    }

    /// <summary>
    /// Returns the specified view bound to the given model type.
    /// </summary>
    /// <param name="modelType">The model type to be used for binding.</param>
    /// <param name="viewName">The view to be returned.</param>
    /// <returns>The <see cref="IViewComponentResult"/>.</returns>
    public virtual async Task<IViewComponentResult> BindView(Type modelType, string viewName)
    {
        object model = await Binder.Bind(modelType, ViewContext).ConfigureAwait(false);
        return View(viewName, model);
    }
}