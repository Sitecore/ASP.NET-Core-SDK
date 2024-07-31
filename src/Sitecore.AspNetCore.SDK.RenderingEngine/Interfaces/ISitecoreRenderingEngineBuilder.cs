using Microsoft.Extensions.DependencyInjection;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

/// <summary>
/// Contract for configuring Sitecore Rendering Engine services.
/// </summary>
public interface ISitecoreRenderingEngineBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where Sitecore Rendering Engine services are configured.
    /// </summary>
    IServiceCollection Services { get; }
}