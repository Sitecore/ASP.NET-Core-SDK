using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Properties;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding;

/// <inheritdoc />
internal class SitecoreViewModelBinder : IViewModelBinder
{
    /// <inheritdoc />
    public virtual async Task<TModel> Bind<TModel>(ViewContext viewContext)
        where TModel : class, new()
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        TModel model = new();

        await Bind(model, viewContext).ConfigureAwait(false);

        return model;
    }

    /// <inheritdoc />
    public async Task Bind<TModel>(TModel model, ViewContext viewContext)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(viewContext);

        ISitecoreRenderingContext? context = viewContext.HttpContext.GetSitecoreRenderingContext();
        ControllerBase controller = context?.Controller ?? throw new NullReferenceException(Resources.Exception_ControllerCannotBeNull);
        await controller.TryUpdateModelAsync(model).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<object> Bind(Type modelType, ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(viewContext);

        object model;

        try
        {
            object? modelInstance = Activator.CreateInstance(modelType);
            model = modelInstance ?? new object();
        }
        catch (TypeLoadException ex)
        {
            throw new ArgumentException(string.Format(Resources.Exception_CannotCreateModelInstance, modelType), ex);
        }

        if (model == null)
        {
            throw new ArgumentException(string.Format(Resources.Exception_CannotCreateModelInstance, modelType));
        }

        await Bind(model, viewContext).ConfigureAwait(false);

        return model;
    }

    /// <inheritdoc />
    public async Task Bind(object model, ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(viewContext);

        ISitecoreRenderingContext? context = viewContext.HttpContext.GetSitecoreRenderingContext();
        ControllerBase controller = context?.Controller ?? throw new NullReferenceException(Resources.Exception_ControllerCannotBeNull);
        await controller.TryUpdateModelAsync(model, model.GetType(), string.Empty).ConfigureAwait(false);
    }
}