using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;

/// <summary>
/// Creates <see cref="SitecoreLayoutModelBinder{SitecoreLayoutResponseBindingSource}"/> instances.
/// </summary>
public class SitecoreLayoutResponseModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.GetModelBinder<SitecoreLayoutResponseBindingSource, SitecoreLayoutResponse>();
    }
}