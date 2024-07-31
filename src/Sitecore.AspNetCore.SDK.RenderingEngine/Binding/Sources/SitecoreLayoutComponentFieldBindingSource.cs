using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Component"/> field data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SitecoreLayoutComponentFieldBindingSource"/> class.
/// </remarks>
/// <param name="id">The binding source ID.</param>
/// <param name="displayName">The display name.</param>
/// <param name="isGreedy">A value indicating whether the source is greedy.</param>
/// <param name="isFromRequest">A value indicating whether the data comes from the HTTP request.</param>
public class SitecoreLayoutComponentFieldBindingSource(string id, string displayName, bool isGreedy, bool isFromRequest)
    : SitecoreLayoutBindingSource(id, displayName, isGreedy, isFromRequest)
{
    private const string BindingSourceId = nameof(Component) + "Field";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutComponentFieldBindingSource"/> class.
    /// </summary>
    public SitecoreLayoutComponentFieldBindingSource()
        : this(BindingSourceId, BindingSourceId, false, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutComponentFieldBindingSource"/> class.
    /// </summary>
    /// <param name="name">The name of the field in the Sitecore component to use for binding.</param>
    public SitecoreLayoutComponentFieldBindingSource(string name)
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

        return context.Component != null ? GetFieldModel(bindingContext, context.Component, route) : null;
    }
}