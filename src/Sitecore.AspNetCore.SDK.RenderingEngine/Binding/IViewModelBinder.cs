using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding;

/// <summary>
/// Contract that allows models to be bound based on a <see cref="ViewContext"/>.
/// </summary>
public interface IViewModelBinder
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TModel"/> and binds properties
    /// based on the given <paramref name="viewContext"/>.
    /// </summary>
    /// <typeparam name="TModel">The instance type to be returned.</typeparam>
    /// <param name="viewContext">The <see cref="ViewContext"/> to bind against.</param>
    /// <returns>A bound instance of <typeparamref name="TModel"/> or default(<typeparamref name="TModel"/>).</returns>
    Task<TModel> Bind<TModel>(ViewContext viewContext)
        where TModel : class, new();

    /// <summary>
    /// Binds properties on the given <paramref name="model"/> based on the given
    /// <paramref name="viewContext"/>.
    /// </summary>
    /// <typeparam name="TModel">The instance type to be returned.</typeparam>
    /// <param name="model">Ths instance to be bound.</param>
    /// <param name="viewContext">The <see cref="ViewContext"/> to bind against.</param>
    /// <returns>The instance of <paramref name="model"/> with its properties updated.</returns>
    Task Bind<TModel>(TModel model, ViewContext viewContext)
        where TModel : class;

    /// <summary>
    /// Binds properties on the given <paramref name="modelType"/> based on the given
    /// <paramref name="viewContext"/>.
    /// </summary>
    /// <param name="modelType">The instance type to be returned.</param>
    /// <param name="viewContext">The <see cref="ViewContext"/> to bind against.</param>
    /// <returns>The instance of <paramref name="modelType"/> with its properties updated.</returns>
    Task<object> Bind(Type modelType, ViewContext viewContext);

    /// <summary>
    /// Binds properties on the given <paramref name="model"/> based on the given
    /// <paramref name="viewContext"/>.
    /// </summary>
    /// <param name="model">The instance type to be returned.</param>
    /// <param name="viewContext">The <see cref="ViewContext"/> to bind against.</param>
    /// <returns>The instance of <paramref name="model"/> with its properties updated.</returns>
    Task Bind(object model, ViewContext viewContext);
}