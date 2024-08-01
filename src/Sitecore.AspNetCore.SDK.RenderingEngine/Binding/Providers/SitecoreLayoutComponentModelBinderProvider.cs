using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;

/// <summary>
/// Creates <see cref="SitecoreLayoutModelBinder{SitecoreLayoutComponentBindingSource}"/>,
/// <see cref="SitecoreLayoutModelBinder{SitecoreLayoutComponentFieldBindingSource}"/>,
/// <see cref="SitecoreLayoutModelBinder{SitecoreLayoutComponentFieldsBindingSource}"/>,
/// <see cref="SitecoreLayoutModelBinder{SitecoreLayoutComponentPropertyBindingSource}"/> and
/// <see cref="SitecoreLayoutModelBinder{SitecoreLayoutComponentParameterBindingSource}"/> instances.
/// </summary>
public class SitecoreLayoutComponentModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        BinderTypeModelBinder? binder = (context.GetModelBinder<SitecoreLayoutComponentBindingSource, Component>() ??
                                         context.GetModelBinder<SitecoreLayoutComponentFieldsBindingSource, Component>() ??
                                         context.GetModelBinder<SitecoreLayoutComponentFieldBindingSource, IField>()) ??
                                        context.GetModelBinder<SitecoreLayoutComponentPropertyBindingSource>() ??
                                        context.GetModelBinder<SitecoreLayoutComponentParameterBindingSource>();

        return binder;
    }
}