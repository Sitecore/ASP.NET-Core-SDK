using Microsoft.AspNetCore.Builder;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;

/// <summary>
/// Exposes logic to configure Rendering Engine features for a request.
/// </summary>
public class RenderingEnginePipeline
{
    /// <summary>
    /// Adds the Sitecore Rendering Engine features to the given <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to add features to.</param>
    public virtual void Configure(IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        app.UseSitecoreRenderingEngine();
    }
}