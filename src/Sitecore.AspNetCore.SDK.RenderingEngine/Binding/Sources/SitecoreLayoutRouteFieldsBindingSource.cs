using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Route"/> fields.
/// </summary>
public class SitecoreLayoutRouteFieldsBindingSource : SitecoreLayoutBindingSource
{
    private const string BindingSourceId = nameof(Route) + "Fields";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutRouteFieldsBindingSource"/> class.
    /// </summary>
    public SitecoreLayoutRouteFieldsBindingSource()
        : base(nameof(BindingSourceId), nameof(BindingSourceId), false, false)
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

        if (route != null && route.TryReadFields(modelType, out object? result))
        {
            return result;
        }

        return null;
    }
}