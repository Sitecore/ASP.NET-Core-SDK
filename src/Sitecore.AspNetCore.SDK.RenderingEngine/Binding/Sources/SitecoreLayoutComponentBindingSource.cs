using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Component"/> data.
/// </summary>
public class SitecoreLayoutComponentBindingSource : SitecoreLayoutBindingSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutComponentBindingSource"/> class.
    /// </summary>
    public SitecoreLayoutComponentBindingSource()
        : base(nameof(Component), nameof(Component), false, false)
    {
    }

    /// <inheritdoc/>
    public override object? GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext context)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(context);

        Type modelType = bindingContext.ModelMetadata.ModelType;

        if (context.Component != null && modelType == typeof(Component))
        {
            return context.Component;
        }

        return null;
    }
}