using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Component"/> fields.
/// </summary>
public class SitecoreLayoutComponentFieldsBindingSource : SitecoreLayoutBindingSource
{
    private const string BindingSourceId = nameof(Component) + "Fields";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutComponentFieldsBindingSource"/> class.
    /// </summary>
    public SitecoreLayoutComponentFieldsBindingSource()
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

        if (context.Component != null && context.Component.TryReadFields(modelType, out object? result))
        {
            return result;
        }

        return null;
    }
}