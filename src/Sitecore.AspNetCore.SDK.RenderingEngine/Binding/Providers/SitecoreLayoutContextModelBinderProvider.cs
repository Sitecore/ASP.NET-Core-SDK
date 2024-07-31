using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;

/// <summary>
/// Creates <see cref="SitecoreLayoutModelBinder{SitecoreLayoutContextBindingSource}"/> and <see cref="SitecoreLayoutModelBinder{SitecoreLayoutContextPropertyBindingSource}"/> instances.
/// </summary>
public class SitecoreLayoutContextModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        BinderTypeModelBinder? binder = context.GetModelBinder<SitecoreLayoutContextBindingSource, Context>() ??
                                        context.GetModelBinder<SitecoreLayoutContextPropertyBindingSource>();
        return binder;
    }
}