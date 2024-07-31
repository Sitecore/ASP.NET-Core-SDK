using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Route"/> field data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SitecoreLayoutRouteFieldBindingSource"/> class.
/// </remarks>
/// <param name="id">The binding source ID.</param>
/// <param name="displayName">The display name.</param>
/// <param name="isGreedy">A value indicating whether the source is greedy.</param>
/// <param name="isFromRequest">A value indicating whether the data comes from the HTTP request.</param>
public class SitecoreLayoutRouteFieldBindingSource(string id, string displayName, bool isGreedy, bool isFromRequest)
    : SitecoreLayoutBindingSource(id, displayName, isGreedy, isFromRequest)
{
    private const string BindingSourceId = nameof(Route) + "Field";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutRouteFieldBindingSource"/> class.
    /// </summary>
    /// <param name="name">The name of the field in the Sitecore route to use for binding.</param>
    public SitecoreLayoutRouteFieldBindingSource(string name)
        : this(BindingSourceId, BindingSourceId, false, false)
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

        return route != null ? GetFieldModel(bindingContext, route, null) : null;
    }
}