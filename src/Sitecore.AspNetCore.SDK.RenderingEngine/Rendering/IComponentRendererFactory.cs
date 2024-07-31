using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// Creates the appropriate instance of an <see cref="IComponentRenderer"/> for a given <see cref="Component"/>.
/// </summary>
public interface IComponentRendererFactory
{
    /// <summary>
    /// Retrieves an <see cref="IComponentRenderer"/> that has been configured to render the specified <see cref="Component"/>.
    /// </summary>
    /// <param name="component">The <see cref="Component"/> that requires rendering.</param>
    /// <returns>An instance of an <see cref="IComponentRenderer"/> that has been configured to render the component.</returns>
    IComponentRenderer GetRenderer(Component component);
}