using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding <see cref="SitecoreLayoutResponse"/> data.
/// </summary>
public class SitecoreLayoutResponseBindingSource : SitecoreLayoutBindingSource
{
    private const string BindingSourceId = "LayoutServiceResponse";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutResponseBindingSource"/> class.
    /// </summary>
    public SitecoreLayoutResponseBindingSource()
        : base(BindingSourceId, BindingSourceId, false, false)
    {
    }

    /// <inheritdoc/>
    public override object? GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext context)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(context);

        Type modelType = bindingContext.ModelMetadata.ModelType;

        if (modelType == typeof(SitecoreLayoutResponse))
        {
            return context.Response;
        }

        return null;
    }
}