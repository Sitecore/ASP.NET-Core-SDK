using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Route"/> data.
/// </summary>
public class SitecoreLayoutRouteBindingSource : SitecoreLayoutBindingSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutRouteBindingSource"/> class.
    /// </summary>
    public SitecoreLayoutRouteBindingSource()
        : base(nameof(Route), nameof(Route), false, false)
    {
    }

    /// <inheritdoc/>
    public override object? GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext context)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(context);

        Type modelType = bindingContext.ModelMetadata.ModelType;
        Route? route = context.Response?.Content?.Sitecore?.Route;

        if (route != null && modelType == typeof(Route))
        {
            return route;
        }

        return null;
    }
}