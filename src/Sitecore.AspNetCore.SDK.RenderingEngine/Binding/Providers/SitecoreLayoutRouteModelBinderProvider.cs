using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;

/// <summary>
/// Creates <see cref="SitecoreLayoutModelBinder{SitecoreLayoutRouteBindingSource}"/>,
/// <see cref="SitecoreLayoutModelBinder{SitecoreLayoutRouteFieldsBindingSource}"/>,
/// <see cref="SitecoreLayoutModelBinder{SitecoreLayoutRouteFieldBindingSource}"/>
/// and <see cref="SitecoreLayoutModelBinder{SitecoreLayoutRoutePropertyBindingSource}"/> instances.
/// </summary>
public class SitecoreLayoutRouteModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        BinderTypeModelBinder? binder = context.GetModelBinder<SitecoreLayoutRouteBindingSource, Route>() ??
                                        context.GetModelBinder<SitecoreLayoutRouteFieldsBindingSource, Route>() ??
                                        context.GetModelBinder<SitecoreLayoutRouteFieldBindingSource>() ??
                                        context.GetModelBinder<SitecoreLayoutRoutePropertyBindingSource>();

        return binder;
    }
}