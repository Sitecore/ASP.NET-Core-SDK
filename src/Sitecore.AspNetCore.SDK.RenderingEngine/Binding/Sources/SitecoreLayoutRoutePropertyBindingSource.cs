using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Route"/> property data.
/// </summary>
public class SitecoreLayoutRoutePropertyBindingSource
    : SitecoreLayoutBindingSource
{
    private const string BindingSourceId = nameof(Route) + "Property";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutRoutePropertyBindingSource"/> class.
    /// </summary>
    /// <param name="name">The name of the property in the Sitecore route to use for binding.</param>
    public SitecoreLayoutRoutePropertyBindingSource(string name)
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

        Route? route = context.Response?.Content?.Sitecore?.Route;

        return route != null ? GetPropertyModel(bindingContext, route) : null;
    }
}