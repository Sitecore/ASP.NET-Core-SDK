using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;

/// <inheritdoc />
/// <summary>
/// Initializes a new instance of the <see cref="SitecoreRenderingEngineBuilder"/> class.
/// </summary>
/// <param name="services">The initial <see cref="IServiceCollection"/>.</param>
public class SitecoreRenderingEngineBuilder(IServiceCollection services)
    : ISitecoreRenderingEngineBuilder
{
    /// <inheritdoc />
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}