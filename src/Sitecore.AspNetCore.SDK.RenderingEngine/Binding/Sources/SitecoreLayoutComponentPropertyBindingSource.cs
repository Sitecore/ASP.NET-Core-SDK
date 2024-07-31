using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Component"/> property data.
/// </summary>
public class SitecoreLayoutComponentPropertyBindingSource : SitecoreLayoutBindingSource
{
    private const string BindingSourceId = nameof(Component) + "Property";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutComponentPropertyBindingSource"/> class.
    /// </summary>
    /// <param name="name">The name of the property in the Sitecore component to use for binding.</param>
    public SitecoreLayoutComponentPropertyBindingSource(string name)
        : base(BindingSourceId, BindingSourceId, false, false)
    {
        Name = name;
    }

    /// <inheritdoc/>
    public override object? GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext context)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(context);

        return context.Component != null ? GetPropertyModel(bindingContext, context.Component) : null;
    }
}