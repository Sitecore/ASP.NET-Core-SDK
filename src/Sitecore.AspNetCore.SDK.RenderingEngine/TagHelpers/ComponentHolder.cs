using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers;

/// <summary>
/// Represents a usage context for a Component.
/// </summary>
internal class ComponentHolder : IDisposable
{
    private readonly ISitecoreRenderingContext _renderingContext;

    private readonly Component? _oldContextComponent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentHolder"/> class.
    /// Starts a new component rendering context.
    /// </summary>
    /// <param name="renderingContext">The Rendering Context.</param>
    /// <param name="component">The Component.</param>
    public ComponentHolder(ISitecoreRenderingContext renderingContext, Component component)
    {
        _renderingContext = renderingContext ?? throw new ArgumentNullException(nameof(renderingContext));
        _oldContextComponent = _renderingContext.Component;
        _renderingContext.Component = component ?? throw new ArgumentNullException(nameof(component));
    }

    /// <summary>
    /// Ends the component rendering context and reinstates the prior context.
    /// </summary>
    public void Dispose()
    {
        _renderingContext.Component = _oldContextComponent;
    }
}