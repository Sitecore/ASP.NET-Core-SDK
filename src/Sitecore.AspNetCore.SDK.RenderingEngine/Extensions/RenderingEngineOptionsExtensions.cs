using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Extensions to help configure <see cref="RenderingEngineOptions"/>.
/// </summary>
public static class RenderingEngineOptionsExtensions
{
    /// <summary>
    /// Maps a Sitecore layout component name to a partial view rendering. The view will receive a <see cref="Component"/> model.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="partialViewPath">The path of the partial view. The file name of the partial view will be the registered Sitecore layout component name.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddPartialView(
        this RenderingEngineOptions options,
        string partialViewPath)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(partialViewPath);

        string layoutComponentName = ExtractComponentNameFromViewPath(partialViewPath);

        return AddPartialView(
            options,
            layoutComponentName,
            partialViewPath);
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a partial view rendering. The view will receive a <see cref="Component"/> model.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="layoutComponentName">The name of the layout component.</param>
    /// <param name="partialViewPath">The path of the partial view.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddPartialView(
        this RenderingEngineOptions options,
        string layoutComponentName,
        string partialViewPath)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(layoutComponentName);
        ArgumentException.ThrowIfNullOrWhiteSpace(partialViewPath);

        return AddPartialView(
            options,
            name => layoutComponentName.Equals(name, StringComparison.OrdinalIgnoreCase),
            partialViewPath);
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a partial view rendering. The view will receive a <see cref="Component"/> model.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="match">The predicate to use when attempting to match a layout component.</param>
    /// <param name="partialViewPath">The path of the partial view.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddPartialView(
        this RenderingEngineOptions options,
        Predicate<string> match,
        string partialViewPath)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(match);
        ArgumentException.ThrowIfNullOrWhiteSpace(partialViewPath);

        ComponentRendererDescriptor descriptor = PartialViewComponentRenderer.Describe(match, partialViewPath);

        options.RendererRegistry.Add(options.RendererRegistry.Count, descriptor);

        return options;
    }

    /// <summary>
    /// Maps any unmatched Sitecore layout component to a default partial view.
    /// This provides a visual appearance when the Sitecore layout service returns a component name that no implementation exists for.
    /// The view will receive a <see cref="Component"/> model.
    /// Ensure this is registered last as this mapping will override any subsequent component registrations.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="partialViewPath">The path of the partial view.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddDefaultPartialView(
        this RenderingEngineOptions options,
        string partialViewPath)
    {
        return AddPartialView(options, _ => true, partialViewPath);
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a view component rendering.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="viewComponentName">The view component name. This is also used as the layout component registration name.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddViewComponent(
        this RenderingEngineOptions options,
        string viewComponentName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewComponentName);

        return AddViewComponent(
            options,
            viewComponentName,
            viewComponentName);
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a view component rendering.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="layoutComponentName">The name of the layout component.</param>
    /// <param name="viewComponentName">The view component name.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddViewComponent(
        this RenderingEngineOptions options,
        string layoutComponentName,
        string viewComponentName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(layoutComponentName);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewComponentName);

        return AddViewComponent(
            options,
            name => layoutComponentName.Equals(name, StringComparison.OrdinalIgnoreCase),
            viewComponentName);
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a view component rendering.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="match">A predicate to use when attempting to match a layout component.</param>
    /// <param name="viewComponentName">The view component name.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddViewComponent(
        this RenderingEngineOptions options,
        Predicate<string> match,
        string viewComponentName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(match);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewComponentName);

        ComponentRendererDescriptor descriptor = ViewComponentComponentRenderer.Describe(match, viewComponentName);

        options.RendererRegistry.Add(options.RendererRegistry.Count, descriptor);

        return options;
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a partial view rendering, using the default Sitecore view component to model bind it.
    /// </summary>
    /// <typeparam name="TModel">The model type to use for view binding.</typeparam>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="viewName">The view name and layout component name. If the view name is a full path, the view's file name will be the layout component name.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddModelBoundView<TModel>(
        this RenderingEngineOptions options,
        string viewName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewName);

        return AddModelBoundView<TModel>(
            options,
            ExtractComponentNameFromViewPath(viewName),
            viewName);
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a partial view rendering, using the default Sitecore view component to model bind it.
    /// </summary>
    /// <typeparam name="TModel">The model type to use for view binding.</typeparam>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="layoutComponentName">The name of the layout component.</param>
    /// <param name="viewName">The view name.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddModelBoundView<TModel>(
        this RenderingEngineOptions options,
        string layoutComponentName,
        string viewName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(layoutComponentName);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewName);

        return AddModelBoundView<TModel>(
            options,
            match => match.Equals(layoutComponentName, StringComparison.OrdinalIgnoreCase),
            viewName);
    }

    /// <summary>
    /// Maps a Sitecore layout component name to a partial view rendering, using the default Sitecore view component to model bind it.
    /// </summary>
    /// <typeparam name="TModel">The model type to use for view binding.</typeparam>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="match">A predicate to use when attempting to match a layout component.</param>
    /// <param name="viewName">The view name.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddModelBoundView<TModel>(
        this RenderingEngineOptions options,
        Predicate<string> match,
        string viewName)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(match);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewName);

        ComponentRendererDescriptor descriptor = new(
            match,
            sp => ActivatorUtilities.CreateInstance<ModelBoundViewComponentComponentRenderer<TModel>>(
                sp,
                RenderingEngineConstants.SitecoreViewComponents.DefaultSitecoreViewComponentName,
                viewName),
            viewName);

        options.RendererRegistry.Add(options.RendererRegistry.Count, descriptor);

        return options;
    }

    /// <summary>
    /// Maps a default <see cref="IComponentRenderer"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IComponentRenderer"/> to use for the default renderer.</typeparam>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddDefaultComponentRenderer<T>(
        this RenderingEngineOptions options)
        where T : IComponentRenderer
    {
        ArgumentNullException.ThrowIfNull(options);

        ComponentRendererDescriptor descriptor = new(_ => true, services => ActivatorUtilities.CreateInstance<T>(services), "defaultComponent");
        options.DefaultRenderer = descriptor;

        return options;
    }

    /// <summary>
    /// Maps a default <see cref="IComponentRenderer"/>.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddDefaultComponentRenderer(
        this RenderingEngineOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return options.AddDefaultComponentRenderer<LoggingComponentRenderer>();
    }

    /// <summary>
    /// Adds <see cref="SitecoreLayoutRequest"/> mapping action.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="mapAction">The mapping action to configure <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions MapToRequest(this RenderingEngineOptions options, Action<HttpRequest, SitecoreLayoutRequest> mapAction)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(mapAction);

        options.RequestMappings.Add(mapAction);

        return options;
    }

    /// <summary>
    /// Adds post rendering action to be executed after Rendering engine logic.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> to configure.</param>
    /// <param name="postAction">The action to execute after rendering engine/>.</param>
    /// <returns>The <see cref="RenderingEngineOptions"/> so that additional calls can be chained.</returns>
    public static RenderingEngineOptions AddPostRenderingAction(this RenderingEngineOptions options, Action<HttpContext> postAction)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(postAction);

        options.PostRenderingActions.Add(postAction);

        return options;
    }

    /// <summary>
    /// Extracts the final name of a path that may be full or not to a view.
    /// </summary>
    /// <example>"Foo" => "Foo", "~/bar/Baz.cshtml" => "Baz".</example>
    /// <param name="partialViewPath">The path to the partial view to resolve.</param>
    /// <returns>The file name of the partial view.</returns>
    private static string ExtractComponentNameFromViewPath(string partialViewPath)
    {
        return Path.GetFileNameWithoutExtension(partialViewPath);
    }
}